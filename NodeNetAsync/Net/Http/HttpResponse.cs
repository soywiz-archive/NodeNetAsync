using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net;
using NodeNetAsync.Streams;

namespace NodeNetAsync.Net.Http
{
	public class HttpResponse : IAsyncWriter
	{
		/// <summary>
		/// Headers that will be sent as the Response.
		/// </summary>
		public HttpHeaders Headers = new HttpHeaders();

		/// <summary>
		/// Response Code.
		/// </summary>
		public HttpCode Code = HttpCode.OK;

		/// <summary>
		/// 
		/// </summary>
		public Encoding Encoding = new UTF8Encoding(false);

		/// <summary>
		/// 
		/// </summary>
		public TcpSocket Socket;

		/// <summary>
		/// 
		/// </summary>
		MemoryStream Buffer = new MemoryStream();
		
		/// <summary>
		/// 
		/// </summary>
		public bool Buffering = false;

		/// <summary>
		/// 
		/// </summary>
		public bool IsWebSocket = false;

		/// <summary>
		/// 
		/// </summary>
		public int WebSocketVersion;

		/// <summary>
		/// 
		/// </summary>
		public bool HeadersSent { get; private set; }

		/// <summary>
		/// 
		/// </summary>
		private bool _ChunkedTransferEncoding = true;

		/// <summary>
		/// If Content-Length is setted.
		/// </summary>
		public bool ChunkedTransferEncoding
		{
			get
			{
				return _ChunkedTransferEncoding;
			}
			set
			{
				if (!HeadersSent)
				{
					_ChunkedTransferEncoding = value;
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Client"></param>
		public HttpResponse(TcpSocket Client)
		{
			this.Socket = Client;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		async public Task SendHeadersAsync()
		{
			if (!HeadersSent)
			{
				HeadersSent = true;

				if (ChunkedTransferEncoding)
				{
					Headers["Transfer-Encoding"] = "chunked";
				}
				else
				{
					//Headers.Remove("Transfer-Encoding");
				}

				var HeadersString = "";
				HeadersString += "HTTP/1.1 " + (int)Code + " " + HttpCodeUtils.GetStringFromId(Code) + "\r\n";
				HeadersString += Headers.GetEncodeString() + "\r\n";

				if (Buffering)
				{
					var HeadersByteArray = Encoding.GetBytes(HeadersString);
					Buffer.Write(HeadersByteArray, 0, HeadersByteArray.Length);
				}
				else
				{
					await Socket.WriteAsync(HeadersString, Encoding);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		async public Task EndAsync()
		{
			//if (!IsWebSocket)
			{
				await WriteAsync("");

				if (Buffering)
				{
					await Socket.WriteAsync(Buffer.GetBuffer(), 0, (int)Buffer.Length);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Text"></param>
		/// <returns></returns>
		async public Task WriteAsync(string Text)
		{
			if (!HeadersSent) await SendHeadersAsync();
			//await Client.WriteAsync(Convert.ToString(Encoding.GetByteCount(Text), 16).ToUpper() + "\r\n" + Text + "\r\n", Encoding);
			await WriteAsync(Encoding.GetBytes(Text));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Data"></param>
		/// <param name="Offset"></param>
		/// <param name="Count"></param>
		/// <returns></returns>
		async public Task WriteAsync(byte[] Data, int Offset = 0, int Count = -1)
		{
			if (Count < 0) Count = Data.Length;

			if (!HeadersSent) await SendHeadersAsync();

			if (ChunkedTransferEncoding)
			{
				var DataPre = Encoding.GetBytes(Convert.ToString(Count, 16).ToUpper() + "\r\n");
				var DataPost = Encoding.GetBytes("\r\n");

				if (Buffering)
				{
					Buffer.Write(DataPre, 0, DataPre.Length);
					Buffer.Write(Data, Offset, Count);
					Buffer.Write(DataPost, 0, DataPost.Length);
				}
				else
				{
					var Temp = new byte[DataPre.Length + Count + DataPost.Length];
					Array.Copy(DataPre, 0, Temp, 0, DataPre.Length);
					if (Count > 0) Array.Copy(Data, Offset, Temp, 0 + DataPre.Length, Count);
					Array.Copy(DataPost, 0, Temp, 0 + DataPre.Length + Count, DataPost.Length);
					await Socket.WriteAsync(Temp, 0, (DataPre.Length + Count + DataPost.Length));
				}
			}
			else
			{
				if (Buffering)
				{
					Buffer.Write(Data, Offset, Count);
				}
				else
				{
					await Socket.WriteAsync(Data, Offset, Count);
				}
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="SourceStream"></param>
		/// <returns></returns>
		async public Task CopyFromStreamASync(Stream SourceStream)
		{
			int BufferSize = 1024;
			var Buffer = new byte[BufferSize];
			while (true)
			{
				int Readed = await SourceStream.ReadAsync(Buffer, 0, BufferSize);
				if (Readed <= 0) break;
				await WriteAsync(Buffer, 0, Readed);
				if (SourceStream.Position >= SourceStream.Length) break;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="FileName"></param>
		/// <returns></returns>
		async public Task StreamFileASync(string FileName)
		{
			using (var StreamFile = File.Open(FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				await CopyFromStreamASync(StreamFile);
			}
		}
	}
}
