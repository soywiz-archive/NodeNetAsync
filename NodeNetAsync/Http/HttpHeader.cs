using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Http
{
	public struct HttpHeader
	{
		/// <summary>
		/// 
		/// </summary>
		public string Key;

		/// <summary>
		/// 
		/// </summary>
		public string Value;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Line"></param>
		/// <returns></returns>
		static public HttpHeader Parse(string Line)
		{
			var Parts = Line.Split(new[] { ':' }, 2);
			return new HttpHeader()
			{
				Key = Parts[0],
				Value = Parts[1],
			};
		}

		public override string ToString()
		{
			return Key + ": " + Value;
		}
	}
}
