using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http
{
	public class QueryString
	{
		public string String { get; private set; }
		private Dictionary<string, string> Parts = new Dictionary<string,string>();

		public string this[string Key]
		{
			get
			{
				string Out = null;
				Parts.TryGetValue(Key, out Out);
				return Out;
			}
		}

		private void InternalParse(string String)
		{
			this.String = String;
			var Chunks = String.Split('&');
			foreach (var Chunk in Chunks)
			{
				var Parts = Chunk.Split(new[] { '=' }, 2);
				string Key = "";
				string Value = "";
				if (Parts.Length >= 1) Key = UrlDecode(Parts[0]);
				if (Parts.Length >= 2) Value = UrlDecode(Parts[1]);
				this.Parts[Key] = Value;
			}
		}

		static private string UrlDecode(string String)
		{
			var Out = new byte[String.Length];
			int m = 0;
			for (int n = 0; n < String.Length; n++)
			{
				var Char = String[n];
				if (Char == '%')
				{
					if (n + 3 < String.Length)
					{
						try
						{
							Out[m++] = Convert.ToByte(String.Substring(n + 1, 2), 0x10);
						}
						catch
						{
						}
						n += 2;
						continue;
					}
				}
				Out[m++] = (byte)Char;
			}
			return Encoding.UTF8.GetString(Out, 0, m);
		}

		public QueryString(string String)
		{
			InternalParse(String);
		}

		static public implicit operator QueryString(string String)
		{
			return new QueryString(String);
		}

		static public implicit operator string(QueryString QueryString)
		{
			return QueryString.String;
		}
	}
}
