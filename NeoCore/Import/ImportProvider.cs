using System;
using System.Diagnostics;
using Memkit;
using Memkit.Pointers;
using NeoCore.Utilities.Diagnostics;

namespace NeoCore.Import
{
	/// <summary>
	/// Describes a type that provides methods for use in <see cref="ImportManager"/>.
	/// </summary>
	public abstract class ImportProvider
	{
		protected ImportProvider(ProcessModule module)
		{
			Module      = module;
			BaseAddress = module.BaseAddress;
		}

		public ProcessModule Module      { get; }
		public Pointer<byte> BaseAddress { get; }

		/// <summary>
		/// Retrieves the address of the specified member specified by <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Any identifier for the imported member</param>
		/// <returns>Address of the imported member</returns>
		public abstract Pointer<byte> GetAddress(string id);

		public abstract Pointer<byte>[] GetAddresses(string[] ids);

		/// <summary>
		/// Creates a <typeparamref name="TDelegate"/> from the imported member
		/// specified by <paramref name="id"/>.
		/// </summary>
		/// <param name="id">Any identifier for the imported function</param>
		/// <returns><see cref="Delegate"/> of type <typeparamref name="TDelegate"/></returns>
		public virtual TDelegate GetFunction<TDelegate>(string id) where TDelegate : Delegate
		{
			throw new NotImplementedException();
		}
	}
}