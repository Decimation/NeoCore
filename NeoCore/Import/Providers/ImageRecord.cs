using System.Collections.Generic;

namespace NeoCore.Import.Providers
{
	public class ImageRecord
	{
		public ImageRecordInfo                        Info    { get; set; }
		public Dictionary<string, ImageRecordEntry[]> Records { get; set; }
	}
}