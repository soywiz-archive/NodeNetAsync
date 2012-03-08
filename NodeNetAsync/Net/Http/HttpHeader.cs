using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http
{
	public class HttpHeader
	{
		/// <summary>
		/// 
		/// </summary>
		public string Key { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public string Value { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		public string NormalizedKey { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Key"></param>
		/// <param name="Value"></param>
		public HttpHeader(string Key, string Value)
		{
			this.Key = Key.Trim();
			this.Value = Value.Trim();
			this.NormalizedKey = NormalizeKey(this.Key);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Line"></param>
		/// <returns></returns>
		static public HttpHeader Parse(string Line)
		{
			var Parts = Line.Split(new[] { ':' }, 2);
			return new HttpHeader(Parts[0], Parts[1]);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return Key + ": " + Value;
		}

		public string GetEncodeString()
		{
			return Key + ": " + Value + "\r\n";
		}

		public static string NormalizeKey(string Key)
		{
			return Key.ToLowerInvariant().Trim();
		}
	}
}
