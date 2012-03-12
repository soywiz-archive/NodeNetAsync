using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Utils
{
	public class MimeType
	{
		static public string GetFromPath(string FilePath)
		{
			var Ext = Path.GetExtension(FilePath).ToLowerInvariant();
			if (Ext.Length == 0) return "text/html";

			switch (Ext.Substring(1))
			{
				case "txt": return "text/plain";
				case "js": return "text/javascript";
				case "css": return "text/css";
				case "php":
				case "htm": case "html": return "text/html";
				case "ico": return "image/png";
				case "png": return "image/png";
				case "jpeg": case "jpg": return "image/jpeg";
				default: return "octet/stream";
			}
		}
	}
}
