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
				default:
					await Console.Out.WriteLineAsync("Unknown WebSocketVersion: " + WebSocketVersion);
					throw (new Exception("Unknown WebSocketVersion: " + WebSocketVersion));
					//break;
			}
		}
	}
}
