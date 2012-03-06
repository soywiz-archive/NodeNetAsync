using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http.Router
{
	public class HttpRouter : IHttpFilter
	{
		protected Dictionary<Regex, Func<HttpRequest, HttpResponse, Task>> Routes = new Dictionary<Regex, Func<HttpRequest, HttpResponse, Task>>();

		public void AddRoute(string Path, Func<HttpRequest, HttpResponse, Task> Route)
		{
			Routes.Add(new Regex("^" + Path + "$", RegexOptions.Compiled), Route);
		}

		public void Route(HttpRequest Request, HttpResponse Response)
		{
			foreach (var Route in Routes)
			{
				if (Route.Key.IsMatch(Request.Url))
				{
					Route.Value(Request, Response);
					return;
				}
			}
			throw(new Exception("No route found!"));
		}

		public void Filter(HttpRequest Request, HttpResponse Response)
		{
			Route(Request, Response);
		}
	}
}
