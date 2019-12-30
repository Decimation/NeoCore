using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace NeoCore.Import.Providers
{
	public struct ImageRecord
	{
		public ImageRecordInfo Info { get; set; }

		public Dictionary<string, ImageRecordEntry[]> Records { get; set; }
	}
}