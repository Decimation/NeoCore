#region

using System.Reflection;
using InlineIL;
using Memkit.Model.Attributes;

#endregion

namespace NeoCore.Utilities
{
	#region

	using SAMethodSig = StandAloneMethodSig;
	using CC = CallingConventions;

	#endregion


	public static unsafe partial class Functions
	{
		/// <summary>
		/// Provides methods for calling native functions using the IL <c>calli</c> opcode.
		/// </summary>
		public static class Native
		{
			// CoreCLR seems to use entirely __fastcall
			// todo: does this need to use HasThis when necessary?

			#region Call

			/// <summary>
			///     Calls a native function with the <c>calli</c> instruction.
			/// </summary>
			/// <param name="fn">Function address</param>
			/// <typeparam name="TRet">Return type</typeparam>
			/// <returns>A value of <typeparamref name="TRet" /></returns>
			[NativeFunction]
			public static TRet Call<TRet>(void* fn)
			{
				IL.Emit.Ldarg_0();                                         // Load arg "fn"
				IL.Emit.Conv_I();                                          // Convert arg "fn" to native
				IL.Emit.Calli(new SAMethodSig(CC.Standard,        // Calling convention
				                              new TypeRef(typeof(TRet)))); // Return type
				return IL.Return<TRet>();
			}

			/// <summary>
			///     Calls a native function with the <c>calli</c> instruction.
			/// </summary>
			/// <param name="fn">Function address</param>
			/// <param name="arg1">Argument #1</param>
			/// <typeparam name="TRet">Return type</typeparam>
			/// <typeparam name="T1"><paramref name="arg1" /> type</typeparam>
			/// <returns>A value of <typeparamref name="TRet" /></returns>
			[NativeFunction]
			public static TRet Call<TRet, T1>(void* fn, T1 arg1)
			{
				IL.Emit.Ldarg_1();                                       // Load arg "arg1"
				IL.Emit.Ldarg_0();                                       // Load arg "fn"
				IL.Emit.Conv_I();                                        // Convert arg "fn" to native
				IL.Emit.Calli(new SAMethodSig(CC.Standard,      // Calling convention
				                              new TypeRef(typeof(TRet)), // Return type
				                              new TypeRef(typeof(T1)))); // Arg "arg1" type #1
				return IL.Return<TRet>();
			}

			/// <summary>
			///     Calls a native function with the <c>calli</c> instruction.
			/// </summary>
			/// <param name="fn">Function address</param>
			/// <param name="arg1">Argument #1</param>
			/// <param name="arg2">Argument #2</param>
			/// <typeparam name="TRet">Return type</typeparam>
			/// <typeparam name="T1"><paramref name="arg1" /> type</typeparam>
			/// <typeparam name="T2"><paramref name="arg2" /> type</typeparam>
			/// <returns>A value of <typeparamref name="TRet" /></returns>
			[NativeFunction]
			public static TRet Call<TRet, T1, T2>(void* fn, T1 arg1, T2 arg2)
			{
				IL.Emit.Ldarg_1();                                       // Load arg "arg1"
				IL.Emit.Ldarg_2();                                       // Load arg "arg2"
				IL.Emit.Ldarg_0();                                       // Load arg "fn"
				IL.Emit.Conv_I();                                        // Convert arg "fn" to native
				IL.Emit.Calli(new SAMethodSig(CC.Standard,      // Calling convention
				                              new TypeRef(typeof(TRet)), // Return type
				                              new TypeRef(typeof(T1)),   // Arg "arg1" type #1
				                              new TypeRef(typeof(T2)))); // Arg "arg2" type #2
				return IL.Return<TRet>();
			}

			/// <summary>
			///     Calls a native function with the <c>calli</c> instruction.
			/// </summary>
			/// <param name="fn">Function address</param>
			/// <param name="arg1">Argument #1</param>
			/// <param name="arg2">Argument #2</param>
			/// <param name="arg3">Argument #3</param>
			/// <typeparam name="TRet">Return type</typeparam>
			/// <typeparam name="T1"><paramref name="arg1"/> type</typeparam>
			/// <typeparam name="T2"><paramref name="arg2"/> type</typeparam>
			/// <typeparam name="T3"><paramref name="arg3"/> type</typeparam>
			/// <returns>A value of <typeparamref name="TRet" /></returns>
			[NativeFunction]
			public static TRet Call<TRet, T1, T2, T3>(void* fn, T1 arg1, T2 arg2, T3 arg3)
			{
				IL.Emit.Ldarg_1();                                       // Load arg "arg1"
				IL.Emit.Ldarg_2();                                       // Load arg "arg2"
				IL.Emit.Ldarg_3();                                       // Load arg "arg3"
				IL.Emit.Ldarg_0();                                       // Load arg "fn"
				IL.Emit.Conv_I();                                        // Convert arg "fn" to native
				IL.Emit.Calli(new SAMethodSig(CC.Standard,      // Calling convention
				                              new TypeRef(typeof(TRet)), // Return type
				                              new TypeRef(typeof(T1)),   // Arg "arg1" type #1
				                              new TypeRef(typeof(T2)),   // Arg "arg2" type #2
				                              new TypeRef(typeof(T3)))); // Arg "arg3" type #3
				return IL.Return<TRet>();
			}

			#endregion

			#region CallVoid

			/// <summary>
			///     Calls a native function with the <c>calli</c> instruction.
			/// </summary>
			/// <param name="fn">Function address</param>
			[NativeFunction]
			public static void CallVoid(void* fn)
			{
				IL.Emit.Ldarg_0();                                         // Load arg "fn"
				IL.Emit.Conv_I();                                          // Convert arg "fn" to native
				IL.Emit.Calli(new SAMethodSig(CC.Standard,        // Calling convention
				                              new TypeRef(typeof(void)))); // Return type
			}

			/// <summary>
			///     Calls a native function with the <c>calli</c> instruction.
			/// </summary>
			/// <param name="fn">Function address</param>
			/// <param name="arg1">Argument #1</param>
			/// <typeparam name="T1"><paramref name="arg1" /> type</typeparam>
			[NativeFunction]
			public static void CallVoid<T1>(void* fn, T1 arg1)
			{
				IL.Emit.Ldarg_1();                                       // Load arg "arg1"
				IL.Emit.Ldarg_0();                                       // Load arg "fn"
				IL.Emit.Conv_I();                                        // Convert arg "fn" to native
				IL.Emit.Calli(new SAMethodSig(CC.Standard,      // Calling convention
				                              new TypeRef(typeof(void)), // Return type
				                              new TypeRef(typeof(T1)))); // Arg "arg1" type #1
			}

			/// <summary>
			///     Calls a native function with the <c>calli</c> instruction.
			/// </summary>
			/// <param name="fn">Function address</param>
			/// <param name="arg1">Argument #1</param>
			/// <param name="arg2">Argument #2</param>
			[NativeFunction]
			public static void CallVoid<T1, T2>(void* fn, T1 arg1, T2 arg2)
			{
				IL.Emit.Ldarg_1();                                       // Load arg "arg1"
				IL.Emit.Ldarg_2();                                       // Load arg "arg2"
				IL.Emit.Ldarg_0();                                       // Load arg "fn"
				IL.Emit.Conv_I();                                        // Convert arg "fn" to native
				IL.Emit.Calli(new SAMethodSig(CC.Standard,      // Calling convention
				                              new TypeRef(typeof(void)), // Return type
				                              new TypeRef(typeof(T1)),   // Arg "arg1" type #1
				                              new TypeRef(typeof(T2)))); // Arg "arg2" type #2
			}

			#endregion

			#region CallReturnPointer

			/// <summary>
			///     Calls a native function with the <c>calli</c> instruction.
			/// </summary>
			/// <param name="fn">Function address</param>
			[NativeFunction]
			public static void* CallReturnPointer(void* fn)
			{
				IL.Emit.Ldarg_0();                                          // Load arg "fn"
				IL.Emit.Conv_I();                                           // Convert arg "fn" to native
				IL.Emit.Calli(new SAMethodSig(CC.Standard,			// Calling convention
				                              new TypeRef(typeof(void*)))); // Return type
				return IL.ReturnPointer();
			}

			/// <summary>
			///     Calls a native function with the <c>calli</c> instruction.
			/// </summary>
			/// <param name="fn">Function address</param>
			/// <param name="arg1">Argument #1</param>
			[NativeFunction]
			public static void* CallReturnPointer<T1>(void* fn, T1 arg1)
			{
				IL.Emit.Ldarg_1();                                          // Load arg "arg1"
				IL.Emit.Ldarg_0();                                          // Load arg "fn"
				IL.Emit.Conv_I();                                           // Convert arg "fn" to native
				IL.Emit.Calli(new SAMethodSig(CC.Standard,         // Calling convention
				                              new TypeRef(typeof(void*)),   // Return type
				                              new TypeRef(typeof(void*)))); // Arg "arg1" type #1
				return IL.ReturnPointer();
			}

			/// <summary>
			///     Calls a native function with the <c>calli</c> instruction.
			/// </summary>
			/// <param name="fn">Function address</param>
			/// <param name="arg1">Argument #1</param>
			/// <param name="arg2">Argument #2</param>
			[NativeFunction]
			public static void* CallReturnPointer<T1, T2>(void* fn, T1 arg1, T2 arg2)
			{
				IL.Emit.Ldarg_1();                                        // Load arg "arg1"
				IL.Emit.Ldarg_2();                                        // Load arg "arg2"
				IL.Emit.Ldarg_0();                                        // Load arg "fn"
				IL.Emit.Conv_I();                                         // Convert arg "fn" to native
				IL.Emit.Calli(new SAMethodSig(CC.Standard,       // Calling convention
				                              new TypeRef(typeof(void*)), // Return type
				                              new TypeRef(typeof(T1)),    // Arg "arg1" type #1
				                              new TypeRef(typeof(T2))));  // Arg "arg2" type #2
				return IL.ReturnPointer();
			}

			/// <summary>
			///     Calls a native function with the <c>calli</c> instruction.
			/// </summary>
			/// <param name="fn">Function address</param>
			/// <param name="arg1">Argument #1</param>
			/// <param name="arg2">Argument #2</param>
			/// <param name="arg3">Argument #3</param>
			[NativeFunction]
			public static void* CallReturnPointer<T1, T2, T3>(void* fn, T1 arg1, T2 arg2, T3 arg3)
			{
				IL.Emit.Ldarg_1();                                        // Load arg "arg1"
				IL.Emit.Ldarg_2();                                        // Load arg "arg2"
				IL.Emit.Ldarg_3();                                        // Load arg "arg3"
				IL.Emit.Ldarg_0();                                        // Load arg "fn"
				IL.Emit.Conv_I();                                         // Convert arg "fn" to native
				IL.Emit.Calli(new SAMethodSig(CC.Standard,       // Calling convention
				                              new TypeRef(typeof(void*)), // Return type
				                              new TypeRef(typeof(void*)), // Arg "arg1" type #1
				                              new TypeRef(typeof(void*)), // Arg "arg2" type #2
				                              new TypeRef(typeof(T3))));  // Arg "arg3" type #3
				return IL.ReturnPointer();
			}

			#endregion
		}
	}
}