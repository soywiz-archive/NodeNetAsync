using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http.WebSockets
{
	public interface IHttpWebSocketHandler<TType>
	{
		Task OnOpenAsync(WebSocket<TType> Socket);
		Task OnCloseAsync(WebSocket<TType> Socket);
	}
}
