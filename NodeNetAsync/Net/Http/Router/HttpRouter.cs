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

		async public Task Route(HttpRequest Request, HttpResponse Response)
		{
			foreach (var Route in Routes)
			{
				if (Route.Key.IsMatch(Request.Url))
				{
					await Route.Value(Request, Response);
					return;
				}
			}
			Console.WriteLine("No rute found for '" + Request.Url + "'");
			throw(new Exception("No route found!"));
		}

		async public Task Filter(HttpRequest Request, HttpResponse Response)
		{
			await Route(Request, Response);
		}
	}
}
