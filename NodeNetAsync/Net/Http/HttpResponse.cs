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

		public bool HeadersSent { get; private set; }

		async public Task SendHeadersAsync()
		{
			if (!HeadersSent)
			{
				HeadersSent = true;

				var HeadersString = "HTTP/1.1 200 OK\r\n" + Headers.ToString() + "\r\n";

				if (Buffering)
				{
					var HeadersByteArray = Encoding.GetBytes(HeadersString);
					Buffer.Write(HeadersByteArray, 0, HeadersByteArray.Length);
				}
				else
				{
					await Client.WriteAsync(HeadersString, Encoding);
				}
			}
		}

		async public Task EndAsync()
		{
			await WriteChunkAsync("");

			if (Buffer.Length > 0)
			{
				await Client.WriteAsync(Buffer.GetBuffer(), 0, (int)Buffer.Length);
			}

			await Client.CloseAsync();
		}

		async public Task WriteChunkAsync(string Text)
		{
			if (!HeadersSent) await SendHeadersAsync();
			//await Client.WriteAsync(Convert.ToString(Encoding.GetByteCount(Text), 16).ToUpper() + "\r\n" + Text + "\r\n", Encoding);
			await WriteChunkAsync(Encoding.GetBytes(Text));
		}

		MemoryStream Buffer = new MemoryStream();
		public bool Buffering = false;

		async public Task WriteChunkAsync(byte[] Data, int Offset = 0, int Count = -1)
		{
			if (Count < 0) Count = Data.Length;

			var DataPre = Encoding.GetBytes(Convert.ToString(Count, 16).ToUpper() + "\r\n");
			var DataPost = Encoding.GetBytes("\r\n");

			if (!HeadersSent) await SendHeadersAsync();

			if (Buffering)
			{
				Buffer.Write(Data, Offset, Count);
			}
			else
			{
				var Temp = new byte[Count + 32];
				Array.Copy(DataPre, 0, Temp, 0, DataPre.Length);
				if (Count > 0) Array.Copy(Data, Offset, Temp, 0 + DataPre.Length, Count);
				Array.Copy(DataPost, 0, Temp, 0 + DataPre.Length + Data.Length, DataPost.Length);
				await Client.WriteAsync(Temp, 0, (DataPre.Length + Data.Length + DataPost.Length));
			}
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
