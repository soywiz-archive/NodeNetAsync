using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http.WebSockets
{
	public interface IHttpWebSocketHandler<TType>
	{
		Task OnOpen(WebSocket<TType> Socket);
		Task OnClose(WebSocket<TType> Socket);
	}
}
