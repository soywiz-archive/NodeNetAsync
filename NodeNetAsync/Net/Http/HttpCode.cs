using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http
{
	public class HttpCode
	{
		public enum Ids
		{
			WebSocketProtocolHandshake = 101,
			Ok = 200,
			NotFound = 404,
		}

		static public string GetStringFromId(Ids Id)
		{
			switch (Id)
			{
				case Ids.Ok: return "OK";
				case Ids.NotFound: return "Not Found";
				case Ids.WebSocketProtocolHandshake: return "Web Socket Protocol Handshake";
				default: return "Unknwon";
			}
		}
	}
}
