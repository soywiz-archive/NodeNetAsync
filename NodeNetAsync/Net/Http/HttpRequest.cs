using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http
{
	public class HttpRequest
	{
		/// <summary>
		/// 
		/// </summary>
		public HttpHeaders Headers = new HttpHeaders();

		/// <summary>
		/// 
		/// </summary>
		public string Method;

		/// <summary>
		/// 
		/// </summary>
		public string Url;

		/// <summary>
		/// 
		/// </summary>
		public string HttpVersion;

		/// <summary>
		/// 
		/// </summary>
		public bool Ssl;

		/// <summary>
		/// 
		/// </summary>
		public int ConnectionId;
	}
}
