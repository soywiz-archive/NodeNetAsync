using System;
using System.Collections.Generic;
using System.Diagnostics;
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
			this.ConnectHandler = Handler.OnOpenAsync;
			this.DisconnectHandler = Handler.OnCloseAsync;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Request"></param>
		/// <param name="Response"></param>
		/// <returns></returns>
		async Task IHttpFilter.FilterAsync(HttpRequest Request, HttpResponse Response)
		{
			// https://github.com/einaros/ws
			// Hixie draft 76 (Old and deprecated, but still in use by Safari and Opera. Added to ws version 0.4.2, but server only. Can be disabled by setting the disableHixie option to true.)
			// HyBi drafts 07-12 (Use the option protocolVersion: 8, or argument -p 8 for wscat)
			// HyBi drafts 13-17 (Current default, alternatively option protocolVersion: 13, or argument -p 13 for wscat)
			int WebSocketVersion = 0;
			int.TryParse(Request.Headers["Sec-WebSocket-Version"], out WebSocketVersion);

			var Protocol = Request.Headers["Sec-WebSocket-Protocol"];

			Response.Buffering = false;
			Response.ChunkedTransferEncoding = false;
			Response.IsWebSocket = true;
			Response.WebSocketVersion = WebSocketVersion;

			switch (WebSocketVersion)
			{
				// http://tools.ietf.org/html/rfc6455
				// http://tools.ietf.org/html/draft-ietf-hybi-thewebsocketprotocol-08
				case 8:
				case 13:
					{
						var CombinedKey = Request.Headers["Sec-WebSocket-Key"] + "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
						var ComputedHash = Convert.ToBase64String(SHA1.Create().ComputeHash(Encoding.ASCII.GetBytes(CombinedKey)));

						//Response.SetHttpCode(HttpCode.SWITCHING_PROTOCOLS_101, "Web Socket Protocol Handshake");
						Response.SetHttpCode(HttpCode.SWITCHING_PROTOCOLS_101);
						Response.Headers["Upgrade"] = "websocket";
						Response.Headers["Connection"] = "Upgrade";
						Response.Headers["Sec-WebSocket-Accept"] = ComputedHash;

						if (Protocol != "") Response.Headers["Sec-WebSocket-Protocol"] = Protocol;

						var Origin = (WebSocketVersion < 13)
							? Request.Headers["Sec-WebSocket-Origin"]
							: Request.Headers["Origin"]
						;

						if (Origin != "") Response.Headers["Access-Control-Allow-Origin"] = Origin;

						/*
						//Sec-WebSocket-Location: {protocol}://{host}{resource}
						if (WebSocketVersion < 13)
						{
							Response.Headers["Access-Control-Allow-Origin"] = Request.Headers["Sec-WebSocket-Origin"];
						}
						else
						{
							Response.Headers["Access-Control-Allow-Origin"] = Request.Headers["Origin"];
						}
						Response.Headers["Access-Control-Allow-Credentials"] = "true";
						Response.Headers["Access-Control-Allow-Headers"] = "content-type";
						*/
						await Response.SendHeadersAsync();
					}
					break;
				case 0:
					var Key1String = Request.Headers["Sec-WebSocket-Key1"];
					var Key2String = Request.Headers["Sec-WebSocket-Key2"];
					if (Key1String != "" && Key2String != "")
					{
						Response.SetHttpCode(HttpCode.SWITCHING_PROTOCOLS_101);
						Response.Headers["Upgrade"] = "WebSocket";
						Response.Headers["Connection"] = "Upgrade";
						Response.Headers["Sec-WebSocket-Origin"] = Request.Headers["Origin"];
						Response.Headers["Sec-WebSocket-Location"] = GenerateLocation(Request);

						if (Protocol != "") Response.Headers["Sec-WebSocket-Protocol"] = Protocol;

						var Key3 = new byte[8];
						await Response.Socket.ReadAsync(Key3, 0, 8);

						//Console.WriteLine("-------------------------------------------");

						//Console.WriteLine("Sec-WebSocket-Key1: {0}", Key1String);
						//Console.WriteLine("Sec-WebSocket-Key2: {0}", Key2String);
						//Console.WriteLine("Key3              : {0}", BitConverter.ToString(Key3));

						var GeneratedBytes =
							IntToBigEndian(Draft0Hash(Key1String))
							.Concat(IntToBigEndian(Draft0Hash(Key2String)))
							.Concat(Key3)
							.ToArray()
						;

						var Hash = MD5.Create().ComputeHash(GeneratedBytes);

						//Console.WriteLine("Generated-Concat  : {0}", BitConverter.ToString(GeneratedBytes));
						//Console.WriteLine("Generated-Hash    : {0}", BitConverter.ToString(Hash));

						//Console.WriteLine("-------------------------------------------");

						await Response.WriteAsync(Hash);
					}
					else
					{
						throw(new HttpException(HttpCode.BAD_REQUEST_400));
					}
					break;
				default:
					//foreach (var Header in Request.Headers) Console.WriteLine(Header);
					await Console.Out.WriteLineAsync("Unknown WebSocketVersion: " + WebSocketVersion);
					throw (new Exception("Unknown WebSocketVersion: " + WebSocketVersion));
					//break;
			}

			var WebSocket = new WebSocket<TType>(Request, Response);

			Exception YieldedException = null;
			try
			{
				await this.ConnectHandler(WebSocket);
			}
			catch (IOException)
			{
			}
			catch (Exception Exception)
			{
				YieldedException = Exception;
				if (Debugger.IsAttached)
				{
					Console.Error.WriteLine(Exception);
				}
			}

			await this.DisconnectHandler(WebSocket);
		}

		static private String GenerateLocation(HttpRequest Request)
		{
			var DefaultPort = Request.Ssl ? 443 : 80;
			string Location = "";
			Location += (Request.Ssl ? "wss" : "ws") + "://";
			Location += Request.Headers["Host"];
			if (Request.Port != DefaultPort) Location += ":" + Request.Port;
			Location += Request.Url;
			return Location;
		}

		static private byte[] IntToBigEndian(uint Value)
		{
			return new byte[] {
				(byte)(Value >> 24),
				(byte)(Value >> 16),
				(byte)(Value >>  8),
				(byte)(Value >>  0),
			};
		}

		static private uint Draft0Hash(string Str)
		{
			var NumbersString = "";
			var Spaces = 0;
			foreach (var Char in Str)
			{
				if (Char >= '0' && Char <= '9') NumbersString += Char;
				if (Char == ' ') Spaces++;
			}
			var Numbers = uint.Parse(NumbersString);
			//Console.WriteLine("Numbers: {0}, Spaces: {1}", Numbers, Spaces);
			if (Spaces == 0)
			{
				//Console.WriteLine("Spaces == 0");
				throw (new HttpException(HttpCode.BAD_REQUEST_400));
			}
			if ((Numbers % Spaces) != 0)
			{
				//Console.WriteLine("Numbers % Spaces == 0");
				throw (new HttpException(HttpCode.BAD_REQUEST_400));
			}
			return (uint)(Numbers / Spaces);
		}
	}
}
