using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http
{
	public interface IHttpFilter
	{
		Task FilterAsync(HttpRequest Request, HttpResponse Response);
	}
}
