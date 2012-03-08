using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http.WebSockets
{
	public class HttpWebSocket : IHttpFilter
	{
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
						Response.Code = HttpCode.Ids.WebSocketProtocolHandshake;
						Response.Headers["Upgrade"] = "WebSocket";
						Response.Headers["Connection"] = "Upgrade";
						Response.Headers["Sec-WebSocket-Accept"] = ComputedHash;
						Response.Headers["Access-Control-Allow-Origin"] = Request.Headers["Origin"];
						Response.Headers["Access-Control-Allow-Credentials"] = "true";
						Response.Headers["Access-Control-Allow-Headers"] = "content-type";
						await Response.SendHeadersAsync();
					}
					break;
			}
		}
	}
}
