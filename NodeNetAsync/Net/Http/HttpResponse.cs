using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net;

namespace NodeNetAsync.Net.Http
{
	public class HttpResponse
	{
		/// <summary>
		/// 
		/// </summary>
		public HttpHeaders Headers = new HttpHeaders();

		/// <summary>
		/// 
		/// </summary>
		public int Code;

		/// <summary>
		/// 
		/// </summary>
		public Encoding Encoding = new UTF8Encoding(false);

		private TcpSocket Client;

		public HttpResponse(TcpSocket Client)
		{
			this.Client = Client;
		}

		async public Task EndAsync()
		{
			await WriteChunkAsync("");
			await Client.CloseAsync();
		}

		public bool HeadersSent { get; private set; }

		async public Task SendHeadersAsync()
		{
			if (!HeadersSent)
			{
				HeadersSent = true;
				await Client.WriteAsync("HTTP/1.1 200 OK\r\n" + Headers.ToString() + "\r\n", Encoding);
			}
		}

		async public Task WriteChunkAsync(string Text)
		{
			if (!HeadersSent) await SendHeadersAsync();
			await Client.WriteAsync(Convert.ToString(Encoding.GetByteCount(Text), 16).ToUpper() + "\r\n" + Text + "\r\n", Encoding);
		}
	}
}
