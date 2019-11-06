namespace NeoCore.Utilities
{
	public static class Arrays
	{
		public static string ToString<T>(T[] rg)
		{
			return Format.Collections.SimpleJoin(rg);
		}
	}
}