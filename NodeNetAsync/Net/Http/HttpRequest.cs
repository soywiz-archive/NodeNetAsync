using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http
{
	public class HttpRequest
	{
		public HttpRequest(ushort Port, int ConnectionId, int KeepAliveCount)
		{
			this.Port = Port;
			this.ConnectionId = ConnectionId;
			this.KeepAliveCount = KeepAliveCount;
		}

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
		public string HttpVersion = "1.0";

		/// <summary>
		/// 
		/// </summary>
		public ushort Port;

		/// <summary>
		/// 
		/// </summary>
		public bool Ssl
		{
			get
			{
				return Schema == "https";
			}
		}

		/// <summary>
		/// http or https
		/// </summary>
		public string Schema = "http";

		/// <summary>
		/// 
		/// </summary>
		public int ConnectionId;

		/// <summary>
		/// 
		/// </summary>
		public int KeepAliveCount;
	}
}
