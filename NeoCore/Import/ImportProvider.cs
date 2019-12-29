using System;
using NeoCore.Memory;
using NeoCore.Memory.Pointers;
using NeoCore.Utilities.Diagnostics;

namespace NeoCore.Import
{
	/// <summary>
	/// Describes a type that provides methods for use in <see cref="ImportManager"/>.
	/// </summary>
	public abstract class ImportProvider
    {
	    public Pointer<byte> BaseAddress { get; }

	    protected ImportProvider(Pointer<byte> baseAddr)
	    {
		    Guard.AssertNotNull(baseAddr, nameof(baseAddr));
		    BaseAddress = baseAddr;
	    }
	    
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