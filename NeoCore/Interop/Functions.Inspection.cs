#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using NeoCore.Assets;
using NeoCore.CoreClr.VM.Jit;
using NeoCore.Utilities;
using NeoCore.Utilities.Diagnostics;

// ReSharper disable ConvertSwitchStatementToSwitchExpression
// ReSharper disable SwitchStatementMissingSomeCases

#endregion

namespace NeoCore.Interop
{
	public static partial class Functions
	{
		/// <summary>
		/// Contains utilities for inspecting functions.
		/// </summary>
		public static class Inspection
		{
			/// <summary>
			/// <para>Key: <see cref="OpCode.Value"/></para>
			/// <para>Value: <see cref="OpCode"/></para>
			/// </summary>
			private static readonly Dictionary<short, OpCode> OpCodes;

			/// <summary>
			/// <para>Key: <see cref="OperandType"/></para>
			/// <para>Value: size of operand</para>
			/// </summary>
			private static readonly Dictionary<OperandType, int> OperandBaseSizes;

			static Inspection()
			{
				OpCodes          = GetAllOpCodes();
				OperandBaseSizes = GetOperandBaseSizes();
			}

			public static Instruction[] ReadInstructions(byte[] bytes)
			{
				var instructions = new List<Instruction>();

				int offset = 0;

				const short  CODE    = 0xFE;
				const ushort CODE_OR = 0xFE00;

				while (offset < bytes.Length) {
					var instruction = new Instruction
					{
						Offset = offset
					};

					short code = bytes[offset++];

					if (code == CODE) {
						code = (short) (bytes[offset++] | CODE_OR);
					}

					instruction.OpCode = OpCodes[code];

					var opType = instruction.OpCode.OperandType;

					switch (opType) {
						case OperandType.InlineI:
							int value = BitConverter.ToInt32(bytes, offset);
							instruction.Operand =  value;
							offset              += OperandBaseSizes[opType];
							break;

						case OperandType.InlineI8:
							long lvalue = BitConverter.ToInt64(bytes, offset);
							instruction.Operand =  lvalue;
							offset              += OperandBaseSizes[opType];
							break;

						case OperandType.InlineTok:
						case OperandType.InlineType:
						case OperandType.InlineSig:
						case OperandType.InlineField:
						case OperandType.InlineMethod:
							int token = BitConverter.ToInt32(bytes, offset);
							instruction.Operand =  token;
							offset              += OperandBaseSizes[opType];
							break;


						case OperandType.InlineString:
							int mdString = BitConverter.ToInt32(bytes, offset);
							instruction.Operand =  mdString;
							offset              += OperandBaseSizes[opType];
							break;

						case OperandType.InlineSwitch:
							int count = BitConverter.ToInt32(bytes, offset) + 1;
							offset += OperandBaseSizes[opType] * count;
							break;

						case OperandType.InlineBrTarget:
						case OperandType.InlineNone:
						case OperandType.InlineR:
						case OperandType.InlineVar:
						case OperandType.ShortInlineBrTarget:
						case OperandType.ShortInlineI:
						case OperandType.ShortInlineR:
						case OperandType.ShortInlineVar:
							offset += OperandBaseSizes[opType];
							break;

						default:
							throw new NotImplementedException();
					}

					instructions.Add(instruction);
				}

				return instructions.ToArray();
			}


			// Commit with old ILString:
			// 7bff50a8777f9ff528e381d0b740d7e7bdcb760a
			// https://github.com/GeorgePlotnikov/ClrAnalyzer/blob/master/Win32Native/ildump.h


			private static Dictionary<short, OpCode> GetAllOpCodes()
			{
				FieldInfo[] opCodesFields = typeof(OpCodes).GetFields();

				var opCodes = new Dictionary<short, OpCode>(opCodesFields.Length);

				foreach (var t in opCodesFields) {
					var v = (OpCode) t.GetValue(null);
					opCodes.Add(v.Value, v);
				}

				return opCodes;
			}

			private static Dictionary<OperandType, int> GetOperandBaseSizes()
			{
				var dict = new Dictionary<OperandType, int>();

				var intSize = new[]
				{
					OperandType.InlineField,
					OperandType.InlineMethod,
					OperandType.InlineString,
					OperandType.InlineType,
					OperandType.InlineSig,
					OperandType.InlineTok,
					OperandType.InlineSwitch,
					OperandType.InlineBrTarget,
					OperandType.ShortInlineR,
					OperandType.InlineI
				};


				var longSize = new[]
				{
					OperandType.InlineR,
					OperandType.InlineI8
				};

				var byteSize = new[]
				{
					OperandType.ShortInlineVar,
					OperandType.ShortInlineBrTarget,
					OperandType.ShortInlineI
				};

				dict.AddRange(intSize, sizeof(int));
				dict.AddRange(longSize, sizeof(long));
				dict.Add(OperandType.InlineVar, sizeof(short));
				dict.AddRange(byteSize, sizeof(byte));
				dict.Add(OperandType.InlineNone, 0);

				return dict;
			}

			public static bool FunctionThrows(Action action)
			{
				try {
					action();
					return false;
				}
				catch {
					return true;
				}
			}

			/// <summary>
			///     Determines whether <paramref name="action" /> throws <typeparamref name="TException" /> when
			///     executed.
			/// </summary>
			/// <param name="action">Action to run</param>
			/// <typeparam name="TException">Type of <see cref="Exception" /> to check for</typeparam>
			/// <returns>
			///     <c>true</c> if <paramref name="action" /> throws <typeparamref name="TException" /> when executed;
			///     <c>false</c> otherwise
			/// </returns>
			public static bool FunctionThrows<TException>(Action action) where TException : Exception
			{
				try {
					action();
					return false;
				}
				catch (TException) {
					return true;
				}
			}
		}
	}
}