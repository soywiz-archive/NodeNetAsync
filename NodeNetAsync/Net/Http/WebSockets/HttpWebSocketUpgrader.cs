using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Streams;

namespace NodeNetAsync.Net.Http.WebSockets
{
	public class HttpWebSocketUpgrader : HttpWebSocket<object>
	{
		public HttpWebSocketUpgrader(Func<WebSocket<object>, Task> ConnectHandler, Func<WebSocket<object>, Task> DisconnectHandler)
			: base(ConnectHandler, DisconnectHandler)
		{
		}
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
			var WebSocketVersion = Request.Headers["Sec-WebSocket-Version"];
			switch (WebSocketVersion)
			{
				// http://tools.ietf.org/html/rfc6455
				// http://tools.ietf.org/html/draft-ietf-hybi-thewebsocketprotocol-08
				case "8":
				case "13":
					{
						var CombinedKey = Request.Headers["Sec-WebSocket-Key"] + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
						var ComputedHash = Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(CombinedKey)));
						Response.Buffering = false;
						Response.IsWebSocket = true;
						Response.WebSocketVersion = int.Parse(WebSocketVersion);
						Response.SetHttpCode(HttpCode.SWITCHING_PROTOCOLS_101, "Web Socket Protocol Handshake");
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
#if false
				case "":
					var Key1String = Request.Headers["Sec-WebSocket-Key1"];
					var Key2String = Request.Headers["Sec-WebSocket-Key2"];
					if (Key1String != "" && Key2String != "")
					{
						var Key3 = new byte[8];
						await Response.Socket.ReadAsync(Key3, 0, 8);

						var Hash = MD5.Create().ComputeHash(
							IntToBigEndian(Draft0Hash(Key1String))
							.Concat(IntToBigEndian(Draft0Hash(Key2String)))
							.Concat(Key3).ToArray()
						);

						await Response.Socket.WriteAsync(Hash);

						Response.Buffering = false;
						Response.IsWebSocket = true;
						Response.WebSocketVersion = 0;

						Response.Code = HttpCode.WEB_SOCKET_PROTOCOL_HANDSHAKE;
						Response.CodeString = "WebSocket Protocol Handshake";
						Response.Headers["Upgrade"] = "WebSocket";
						Response.Headers["Connection"] = "Upgrade";
						Response.Headers["Sec-WebSocket-Origin"] = Request.Headers["Origin"];
						Response.Headers["Sec-WebSocket-Location"] = Request.Headers["Origin"];
						Response.Headers["Sec-WebSocket-Protocol"] = "true";
					}
					else
					{
					}
					break;
#endif
				default:
					//foreach (var Header in Request.Headers) Console.WriteLine(Header);
					await Console.Out.WriteLineAsync("Unknown WebSocketVersion: " + WebSocketVersion);
					throw (new Exception("Unknown WebSocketVersion: " + WebSocketVersion));
					//break;
			}
		}

		static public byte[] IntToBigEndian(uint Value)
		{
			return new byte[] {
				(byte)(Value >> 24),
				(byte)(Value >> 16),
				(byte)(Value >>  8),
				(byte)(Value >>  0),
			};
		}

		static public uint Draft0Hash(string Str)
		{
			var Numbers = "";
			var SpaceCount = 0U;
			foreach (var Char in Str)
			{
				if (Char >= '0' && Char <= '9') Numbers += Char;
				if (Char == ' ') SpaceCount++;
			}
			return uint.Parse(Numbers) / SpaceCount;
		}
	}
}
