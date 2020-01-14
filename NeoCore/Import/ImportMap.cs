using System;
using System.Collections.Generic;
using Memkit;
using Memkit.Pointers;
using NeoCore.Utilities;
using NeoCore.Win32;

namespace NeoCore.Import
{
	public sealed unsafe class ImportMap
	{
		public const string FIELD_NAME = "Imports";

		private readonly Dictionary<string, Pointer<byte>> m_imports;

		public ImportMap()
		{
			m_imports = new Dictionary<string, Pointer<byte>>();
		}

		public Pointer<byte> this[string key] {
			get {
				if (!m_imports.ContainsKey(key)) {
					string msg = String.Format("Key not found: {0}; Pairs: {1}", key, m_imports.Count);
					throw new KeyNotFoundException(msg);
				}

				return m_imports[key];
			}
		}

		internal void Add(string key, Pointer<byte> value) => m_imports.Add(key, value);

		internal void Clear() => m_imports.Clear();

		#region Call

		public TRet Call<TRet>(string id) =>
			Functions.Native.Call<TRet>(this[id].ToPointer());

		public TRet Call<TRet, T1>(string id, T1 arg1) =>
			Functions.Native.Call<TRet, T1>(this[id].ToPointer(), arg1);

		public TRet Call<TRet, T1, T2>(string id, T1 arg1, T2 arg2) =>
			Functions.Native.Call<TRet, T1, T2>(this[id].ToPointer(), arg1, arg2);

		public TRet Call<TRet, T1, T2, T3>(string id, T1 arg1, T2 arg2, T3 arg3) =>
			Functions.Native.Call<TRet, T1, T2, T3>(this[id].ToPointer(), arg1, arg2, arg3);

		public void CallVoid<T1>(string id) =>
			Functions.Native.CallVoid(this[id].ToPointer());

		public void CallVoid<T1>(string id, T1 arg1) =>
			Functions.Native.CallVoid(this[id].ToPointer(), arg1);

		public void CallVoid<T1, T2>(string id, T1 arg1, T2 arg2) =>
			Functions.Native.CallVoid(this[id].ToPointer(), arg1, arg2);

		public void* CallReturnPointer(string id) =>
			Functions.Native.CallReturnPointer(this[id].ToPointer());

		public void* CallReturnPointer<T1>(string id, T1 arg1) =>
			Functions.Native.CallReturnPointer(this[id].ToPointer(), arg1);

		public void* CallReturnPointer<T1, T2>(string id, T1 arg1, T2 arg2) =>
			Functions.Native.CallReturnPointer(this[id].ToPointer(), arg1, arg2);

		public void* CallReturnPointer<T1, T2, T3>(string id, T1 arg1, T2 arg2, T3 arg3) =>
			Functions.Native.CallReturnPointer(this[id].ToPointer(), arg1, arg2, arg3);

		#endregion
	}
}