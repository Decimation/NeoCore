#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using NeoCore.CoreClr.Components.VM.Jit;
using NeoCore.Utilities.Diagnostics;

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

			static Inspection()
			{
				OpCodes = GetAllOpCodes();
			}

			public static Instruction[] ReadInstructions(MethodBase methodBase)
			{
				var methodBody = methodBase.GetMethodBody();

				return methodBody == null ? null : ReadInstructions(methodBody.GetILAsByteArray());
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

					int origOffset = offset;

					short code = bytes[offset++];

					if (code == CODE) {
						code = (short) (bytes[offset++] | CODE_OR);
					}

					instruction.OpCode = OpCodes[code];

					switch (instruction.OpCode.OperandType) {
						case OperandType.InlineI:
							int value = BitConverter.ToInt32(bytes, offset);
							instruction.Operand =  value;
							offset              += sizeof(int);
							break;

						case OperandType.ShortInlineR:
						case OperandType.InlineBrTarget:
							offset += sizeof(int);
							break;

						case OperandType.InlineR:
							offset += sizeof(long);
							break;

						case OperandType.InlineI8:
							long lvalue = BitConverter.ToInt64(bytes, offset);
							instruction.Operand =  lvalue;
							offset              += sizeof(long);
							break;

						case OperandType.InlineTok:
						case OperandType.InlineType:
						case OperandType.InlineSig:
						case OperandType.InlineField:
						case OperandType.InlineMethod:
							int token = BitConverter.ToInt32(bytes, offset);
							instruction.Operand =  token;
							offset              += sizeof(int);
							break;

						case OperandType.InlineNone:
							break;

						case OperandType.InlineString:
							int mdString = BitConverter.ToInt32(bytes, offset);

							instruction.Operand =  mdString;
							offset              += sizeof(int);
							break;

						case OperandType.InlineSwitch:
							int count = BitConverter.ToInt32(bytes, offset) + 1;
							offset += sizeof(int) * count;
							break;


						case OperandType.InlineVar:
							offset += sizeof(short);
							break;

						case OperandType.ShortInlineVar:
						case OperandType.ShortInlineBrTarget:
						case OperandType.ShortInlineI:
							offset += sizeof(byte);
							break;
						default: throw new NotImplementedException();
					}


					int size = offset - origOffset;

					byte[] raw = bytes.Skip(origOffset)
					                  .Take(size)
					                  .ToArray();

					instructions.Add(instruction);
				}

				return instructions.ToArray();
			}


			// Commit with old ILString
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