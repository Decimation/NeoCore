using System.IO;
using System.Runtime.InteropServices;

namespace NeoCore.Utilities
{
	internal static class FileSystem
	{
		internal static bool TryGetRuntimeFile(string fileName, out FileInfo file)
		{
			file = GetRuntimeFile(fileName);

			if (file == null || !file.Exists) {
				file = null;
				return false;
			}

			return true;
		}
		
		internal static FileInfo GetRuntimeFile(string fileName)
		{
			string path = RuntimeEnvironment.GetRuntimeDirectory() + fileName;
			var    file = new FileInfo(path);

			return file;
		}
	}
}