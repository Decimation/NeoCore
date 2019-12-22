using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace NeoCore.Utilities
{
	// https://github.com/khalidabuhakmeh/ConsoleTables
	
	internal class ConsoleTable
	{
		internal IList<object>   Columns { get; set; }
		internal IList<object[]> Rows    { get; set; }

		internal ConsoleTableOptions Options     { get; set; }
		internal Type[]              ColumnTypes { get; private set; }

		internal static HashSet<Type> NumericTypes = new HashSet<Type>
		{
			typeof(int), typeof(double), typeof(decimal),
			typeof(long), typeof(short), typeof(sbyte),
			typeof(byte), typeof(ulong), typeof(ushort),
			typeof(uint), typeof(float)
		};

		internal ConsoleTable(params string[] columns)
			: this(new ConsoleTableOptions {Columns = new List<string>(columns)}) { }

		internal ConsoleTable(ConsoleTableOptions options)
		{
			Options = options ?? throw new ArgumentNullException(nameof(options));
			Rows    = new List<object[]>();
			Columns = new List<object>(options.Columns);
		}

		internal ConsoleTable AddColumn(IEnumerable<string> names)
		{
			foreach (var name in names)
				Columns.Add(name);
			return this;
		}

		internal ConsoleTable AddRow(params object[] values)
		{
			if (values == null)
				throw new ArgumentNullException(nameof(values));

			if (!Columns.Any())
				throw new Exception("Please set the columns first");

			if (Columns.Count != values.Length)
				throw new Exception(
					$"The number columns in the row ({Columns.Count}) does not match the values ({values.Length}");

			Rows.Add(values);
			return this;
		}

		internal ConsoleTable Configure(Action<ConsoleTableOptions> action)
		{
			action(Options);
			return this;
		}

		internal static ConsoleTable From<T>(IEnumerable<T> values)
		{
			var table = new ConsoleTable
			{
				ColumnTypes = GetColumnsType<T>().ToArray()
			};

			var columns = GetColumns<T>();

			table.AddColumn(columns);

			foreach (
				var propertyValues
				in values.Select(value => columns.Select(column => GetColumnValue<T>(value, column)))
			) table.AddRow(propertyValues.ToArray());

			return table;
		}

		public override string ToString()
		{
			var builder = new StringBuilder();

			// find the longest column by searching each row
			var columnLengths = ColumnLengths();

			// set right alinment if is a number
			var columnAlignment = Enumerable.Range(0, Columns.Count)
			                                .Select(GetNumberAlignment)
			                                .ToList();

			// create the string format with padding
			var format = Enumerable.Range(0, Columns.Count)
			                       .Select(i => " | {" + i + "," + columnAlignment[i] + columnLengths[i] + "}")
			                       .Aggregate((s, a) => s + a) + " |";

			// find the longest formatted line
			var maxRowLength  = Math.Max(0, Rows.Any() ? Rows.Max(row => string.Format(format, row).Length) : 0);
			var columnHeaders = string.Format(format, Columns.ToArray());

			// longest line is greater of formatted columnHeader and longest row
			var longestLine = Math.Max(maxRowLength, columnHeaders.Length);

			// add each row
			var results = Rows.Select(row => string.Format(format, row)).ToList();

			// create the divider
			var divider = " " + string.Join("", Enumerable.Repeat("-", longestLine - 1)) + " ";

			builder.AppendLine(divider);
			builder.AppendLine(columnHeaders);

			foreach (var row in results) {
				builder.AppendLine(divider);
				builder.AppendLine(row);
			}

			builder.AppendLine(divider);

			if (Options.EnableCount) {
				builder.AppendLine("");
				builder.AppendFormat(" Count: {0}", Rows.Count);
			}

			return builder.ToString();
		}

		internal string ToMarkDownString()
		{
			return ToMarkDownString('|');
		}

		private string ToMarkDownString(char delimiter)
		{
			var builder = new StringBuilder();

			// find the longest column by searching each row
			var columnLengths = ColumnLengths();

			// create the string format with padding
			var format = Format(columnLengths, delimiter);

			// find the longest formatted line
			var columnHeaders = string.Format(format, Columns.ToArray());

			// add each row
			var results = Rows.Select(row => string.Format(format, row)).ToList();

			// create the divider
			var divider = Regex.Replace(columnHeaders, @"[^|]", "-");

			builder.AppendLine(columnHeaders);
			builder.AppendLine(divider);
			results.ForEach(row => builder.AppendLine(row));

			return builder.ToString();
		}

		internal string ToMinimalString()
		{
			return ToMarkDownString(char.MinValue);
		}

		internal string ToStringAlternative()
		{
			var builder = new StringBuilder();

			// find the longest column by searching each row
			var columnLengths = ColumnLengths();

			// create the string format with padding
			var format = Format(columnLengths);

			// find the longest formatted line
			var columnHeaders = string.Format(format, Columns.ToArray());

			// add each row
			var results = Rows.Select(row => string.Format(format, row)).ToList();

			// create the divider
			var divider     = Regex.Replace(columnHeaders, @"[^|]", "-");
			var dividerPlus = divider.Replace("|", "+");

			builder.AppendLine(dividerPlus);
			builder.AppendLine(columnHeaders);

			foreach (var row in results) {
				builder.AppendLine(dividerPlus);
				builder.AppendLine(row);
			}

			builder.AppendLine(dividerPlus);

			return builder.ToString();
		}

		private string Format(List<int> columnLengths, char delimiter = '|')
		{
			// set right alinment if is a number
			var columnAlignment = Enumerable.Range(0, Columns.Count)
			                                .Select(GetNumberAlignment)
			                                .ToList();

			var delimiterStr = delimiter == char.MinValue ? string.Empty : delimiter.ToString();
			var format = (Enumerable.Range(0, Columns.Count)
			                        .Select(i => " " + delimiterStr + " {" + i + "," + columnAlignment[i] +
			                                     columnLengths[i] + "}")
			                        .Aggregate((s, a) => s + a) + " " + delimiterStr).Trim();
			return format;
		}

		private string GetNumberAlignment(int i)
		{
			return Options.NumberAlignment == TableAlignment.Right
			       && ColumnTypes != null
			       && NumericTypes.Contains(ColumnTypes[i])
				? ""
				: "-";
		}

		private List<int> ColumnLengths()
		{
			var columnLengths = Columns
			                   .Select((t, i) => Rows.Select(x => x[i])
			                                         .Union(new[] {Columns[i]})
			                                         .Where(x => x != null)
			                                         .Select(x => x.ToString().Length).Max())
			                   .ToList();
			return columnLengths;
		}

		internal void Write(TableFormat format = TableFormat.Default)
		{
			switch (format) {
				case TableFormat.Default:
					Options.OutputTo.WriteLine(ToString());
					break;
				case TableFormat.MarkDown:
					Options.OutputTo.WriteLine(ToMarkDownString());
					break;
				case TableFormat.Alternative:
					Options.OutputTo.WriteLine(ToStringAlternative());
					break;
				case TableFormat.Minimal:
					Options.OutputTo.WriteLine(ToMinimalString());
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(format), format, null);
			}
		}

		private static IEnumerable<string> GetColumns<T>()
		{
			return typeof(T).GetProperties().Select(x => x.Name).ToArray();
		}

		private static object GetColumnValue<T>(object target, string column)
		{
			return typeof(T).GetProperty(column).GetValue(target, null);
		}

		private static IEnumerable<Type> GetColumnsType<T>()
		{
			return typeof(T).GetProperties().Select(x => x.PropertyType).ToArray();
		}
	}

	internal class ConsoleTableOptions
	{
		internal IEnumerable<string> Columns     { get; set; } = new List<string>();
		internal bool                EnableCount { get; set; } = true;

		/// <summary>
		/// Enable only from a list of objects
		/// </summary>
		internal TableAlignment NumberAlignment { get; set; } = TableAlignment.Left;

		/// <summary>
		/// The <see cref="TextWriter"/> to write to. Defaults to <see cref="Console.Out"/>.
		/// </summary>
		internal TextWriter OutputTo { get; set; } = Console.Out;
	}

	internal enum TableFormat
	{
		Default     = 0,
		MarkDown    = 1,
		Alternative = 2,
		Minimal     = 3
	}

	internal enum TableAlignment
	{
		Left,
		Right
	}
}