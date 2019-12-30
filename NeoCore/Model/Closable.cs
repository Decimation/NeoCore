using System;

namespace NeoCore.Model
{
	/// <summary>
	/// Describes a type that must be closed after usage. Implements <see cref="IDisposable"/>.
	/// </summary>
	public abstract class Closable : IDisposable
	{
		protected abstract string Id { get; }

		public void Dispose() => Close();

		public virtual void Close() { }
	}
}