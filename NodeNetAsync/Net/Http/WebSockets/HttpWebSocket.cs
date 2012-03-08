using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http.WebSockets
{
	public class HttpWebSocket : HttpWebSocket<object>
	{
		public HttpWebSocket(Func<WebSocket<object>, Task> ConnectHandler, Func<WebSocket<object>, Task> DisconnectHandler)
			: base(ConnectHandler, DisconnectHandler)
		{
		}
	}

	public interface IHttpWebSocketHandler<TType>
	{
		Task OnOpen(WebSocket<TType> Socket);
		Task OnClose(WebSocket<TType> Socket);
	}

	/// <summary>
	/// 
	/// </summary>
	/// <see cref="http://tools.ietf.org/html/rfc6455"/>
	public class HttpWebSocket<TType> : IHttpFilter
	{
		Func<WebSocket<TType>, Task> ConnectHandler;
		private Func<WebSocket<TType>, Task> DisconnectHandler;

		public HttpWebSocket(Func<WebSocket<TType>, Task> ConnectHandler, Func<WebSocket<TType>, Task> DisconnectHandler)
		{
			this.ConnectHandler = ConnectHandler;
			this.DisconnectHandler = DisconnectHandler;
		}

		public HttpWebSocket(IHttpWebSocketHandler<TType> Handler)
		{
			this.ConnectHandler = Handler.OnOpen;
			this.DisconnectHandler = Handler.OnClose;
		}

		async Task IHttpFilter.Filter(HttpRequest Request, HttpResponse Response)
		{
			switch (Request.Headers["Sec-WebSocket-Version"])
			{
				case "13":
					{
						var CombinedKey = Request.Headers["Sec-WebSocket-Key"] + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
						var ComputedHash = Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(CombinedKey)));
						Response.Buffering = false;
						Response.IsWebSocket = true;
						Response.WebSocketVersion = 13;
						Response.Code = HttpCode.WEB_SOCKET_PROTOCOL_HANDSHAKE;
						Response.Headers["Upgrade"] = "WebSocket";
						Response.Headers["Connection"] = "Upgrade";
						Response.Headers["Sec-WebSocket-Accept"] = ComputedHash;
						Response.Headers["Access-Control-Allow-Origin"] = Request.Headers["Origin"];
						Response.Headers["Access-Control-Allow-Credentials"] = "true";
						Response.Headers["Access-Control-Allow-Headers"] = "content-type";
						await Response.SendHeadersAsync();


						var WebSocket = new WebSocket<TType>(Request, Response);

						Exception YieldedException = null;
						try
						{
							await this.ConnectHandler(WebSocket);
						}
						catch (Exception Exception)
						{
							YieldedException = Exception;
							//Console.Error.WriteLine(Exception);
						}

						await this.DisconnectHandler(WebSocket);
					}
					break;
			}
		}
	}

	public class WebSocket<TType>
	{
		static internal int LastUniqueId = 0;
		public int UniqueId;
		public HttpRequest HttpRequest;
		public HttpResponse HttpResponse;
		public TcpSocket Socket;
		public enum OpcodeEnum : byte
		{
			ContinuationFrame = 0,
			TextFrame = 1,
			BinaryFrame = 2,
			ConnectionClose = 8,
			Ping = 9,
			Pong = 10,
		}
		public Encoding DefaultEncoding = Encoding.UTF8;
		public TType Tag;

		internal WebSocket(HttpRequest Request, HttpResponse Response)
		{
			this.HttpRequest = Request;
			this.HttpResponse = Response;
			this.Socket = Response.Socket;
			this.UniqueId = LastUniqueId++;
		}

		async public Task<string> ReadPacketAsStringAsync()
		{
			return DefaultEncoding.GetString(await ReadPacketAsync());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <see cref="5.2.  Base Framing Protocol"/>
		async public Task<byte[]> ReadPacketAsync()
		{
			var Header = new byte[2];
			var Mask = new byte[4];
			var Data = new MemoryStream();
			var Temp = new byte[128];
			OpcodeEnum Opcode;
			bool IsFinal;
			bool IsMasked;
			int PayloadLength;

			do
			{
				await Socket.ReadAsync(Header, 0, 2);
				IsFinal = (((Header[0] >> 7) & 0x1) != 0);
				Opcode = (OpcodeEnum)((Header[0] >> 0) & 0x7);
				PayloadLength = (Header[1] >> 0) & 0x7F;
				IsMasked = ((Header[1] >> 7) & 0x1) != 0;
				if (IsMasked)
				{
					await Socket.ReadAsync(Mask, 0, 4);
				}
				await Socket.ReadAsync(Temp, 0, PayloadLength);
				for (int n = 0; n < PayloadLength; n++) Temp[n] ^= Mask[n % 4];
				Data.Write(Temp, 0, PayloadLength);
			} while (!IsFinal);

			return Data.GetContentBytes();
		}

		async public Task WritePacketAsync(string Text, Encoding Encoding = null)
		{
			if (Encoding == null) Encoding = DefaultEncoding;
			await WritePacketAsync(Encoding.GetBytes(Text));
		}

		async public Task WritePacketAsync(byte[] Data, int Offset = 0, int Count = -1)
		{
			if (Count == -1) Count = Data.Length;
			var MemoryStream = new MemoryStream();

			do
			{
				int ChunkSize = Math.Min(Count, 127);
				bool IsFinal = (ChunkSize == Count);

				MemoryStream.WriteByte((byte)((int)(OpcodeEnum.TextFrame) | (int)(IsFinal ? 0x80 : 0x00)));
				MemoryStream.WriteByte((byte)ChunkSize);
				MemoryStream.Write(Data, Offset, ChunkSize);

				Offset += ChunkSize;
				Count -= ChunkSize;
			} while (Count > 0);

			await Socket.WriteAsync(MemoryStream.GetBuffer(), 0, (int)MemoryStream.Length);
		}
	}
}
