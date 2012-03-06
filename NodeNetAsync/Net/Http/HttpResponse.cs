using System;
using System.Collections.Generic;
using System.IO;
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
			//await Client.WriteAsync(Convert.ToString(Encoding.GetByteCount(Text), 16).ToUpper() + "\r\n" + Text + "\r\n", Encoding);
			await WriteChunkAsync(Encoding.GetBytes(Text));
		}

		async public Task WriteChunkAsync(byte[] Data, int Offset = 0, int Count = -1)
		{
			if (Count < 0) Count = Data.Length;
			if (!HeadersSent) await SendHeadersAsync();

#if true
			await Client.WriteAsync(Encoding.GetBytes(Convert.ToString(Count, 16).ToUpper() + "\r\n"));
			if (Count > 0)
			{
				await Client.WriteAsync(Data, Offset, Count);
			}
			await Client.WriteAsync(Encoding.GetBytes("\r\n"));
#else
			//await Client.WriteAsync(Encoding.GetBytes(Convert.ToString(Data.Length, 16).ToUpper() + "\r\n").Concat(Data).Concat(Encoding.GetBytes("\r\n")).ToArray());
#endif
		}

		async public Task CopyFromStreamASync(Stream SourceStream)
		{
			int BufferSize = 1024;
			var Buffer = new byte[BufferSize];
			while (true)
			{
				int Readed = await SourceStream.ReadAsync(Buffer, 0, BufferSize);
				if (Readed <= 0) break;
				await WriteChunkAsync(Buffer, 0, Readed);
				if (SourceStream.Position >= SourceStream.Length) break;
			}
		}
	}
}
