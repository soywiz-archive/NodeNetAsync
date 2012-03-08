using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http
{
	public enum HttpCode
	{
		WEB_SOCKET_PROTOCOL_HANDSHAKE = 101,
		OK = 200,
		NOT_FOUND = 404,
	}

	public class HttpCodeUtils
	{
		static public string GetStringFromId(HttpCode Code)
		{
			switch (Code)
			{
				case HttpCode.OK: return "OK";
				case HttpCode.NOT_FOUND: return "Not Found";
				case HttpCode.WEB_SOCKET_PROTOCOL_HANDSHAKE: return "Web Socket Protocol Handshake";
				default: return "Unknwon";
			}
		}
	}
}
