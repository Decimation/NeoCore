namespace NeoCore.Model
{
	public abstract class Allocator : Releasable
	{
		public int AllocCount { get; }
		
		public bool IsMemoryInUse => AllocCount > 0;
	}
}