using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using NeoCore.Utilities;
using SimpleCore;
using SimpleCore.Formatting;

namespace NeoCore.CoreClr.VM.Jit
{
	/// <summary>
	/// Represents an IL instruction.
	/// </summary>
	public struct Instruction
	{
		public int Offset { get; internal set; }

		public OpCode OpCode { get; internal set; }

		public object Operand { get; internal set; }

		public bool IsMethodCall => Operand is MethodInfo;

		public bool IsConstructorCall => Operand is MethodInfo m && m.IsConstructor;

		/// <summary>
		/// <para>Key: <see cref="OpCode.Value"/></para>
		/// <para>Value: <see cref="OpCode"/></para>
		/// </summary>
		private static Dictionary<short, OpCode> AllOpCodes { get; } = GetAllOpCodes();

		/// <summary>
		/// <para>Key: <see cref="OperandType"/></para>
		/// <para>Value: size of operand</para>
		/// </summary>
		private static Dictionary<OperandType, int> AllOperandBaseSizes { get; } = GetOperandBaseSizes();

		public override string ToString()
		{
			string dataString;

			if (Operand != null) {
				dataString = Format.ToHexString(Operand);
			}
			else {
				dataString = String.Empty;
			}

			return String.Format("IL_{0:X}: {1} (opcode: {2:X}) {3}", Offset, OpCode, OpCode.Value, dataString);

//			return String.Format("IL_{0:X}: {1} ({2:X}, {3}) {4}", Offset, OpCode, OpCode.Value,OpCode.OperandType , dataString);
		}

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

			AddRange(intSize, sizeof(int));
			AddRange(longSize, sizeof(long));
			dict.Add(OperandType.InlineVar, sizeof(short));
			AddRange(byteSize, sizeof(byte));
			dict.Add(OperandType.InlineNone, 0);

			void AddRange(IEnumerable<OperandType> keys, int value)
			{
				foreach (var key in keys) {
					dict.Add(key, value);
				}
			}

			return dict;
		}

		public static Instruction[] ReadInstructions(byte[] bytes)
		{
			// Commit with old ILString:
			// 7bff50a8777f9ff528e381d0b740d7e7bdcb760a
			// https://github.com/GeorgePlotnikov/ClrAnalyzer/blob/master/Win32Native/ildump.h

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

				instruction.OpCode = AllOpCodes[code];

				var opType = instruction.OpCode.OperandType;

				switch (opType) {
					case OperandType.InlineI:
						int value = BitConverter.ToInt32(bytes, offset);
						instruction.Operand =  value;
						offset              += AllOperandBaseSizes[opType];
						break;

					case OperandType.InlineI8:
						long lvalue = BitConverter.ToInt64(bytes, offset);
						instruction.Operand =  lvalue;
						offset              += AllOperandBaseSizes[opType];
						break;

					case OperandType.InlineTok:
					case OperandType.InlineType:
					case OperandType.InlineSig:
					case OperandType.InlineField:
					case OperandType.InlineMethod:
						int token = BitConverter.ToInt32(bytes, offset);
						instruction.Operand =  token;
						offset              += AllOperandBaseSizes[opType];
						break;


					case OperandType.InlineString:
						int mdString = BitConverter.ToInt32(bytes, offset);
						instruction.Operand =  mdString;
						offset              += AllOperandBaseSizes[opType];
						break;

					case OperandType.InlineSwitch:
						int count = BitConverter.ToInt32(bytes, offset) + 1;
						offset += AllOperandBaseSizes[opType] * count;
						break;

					case OperandType.InlineBrTarget:
					case OperandType.InlineNone:
					case OperandType.InlineR:
					case OperandType.InlineVar:
					case OperandType.ShortInlineBrTarget:
					case OperandType.ShortInlineI:
					case OperandType.ShortInlineR:
					case OperandType.ShortInlineVar:
						offset += AllOperandBaseSizes[opType];
						break;

					default:
						throw new NotImplementedException();
				}

				instructions.Add(instruction);
			}

			return instructions.ToArray();
		}
	}
}