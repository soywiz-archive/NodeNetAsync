﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NodeNetAsync.Net.Http.Router
{
	public class HttpRouter : IHttpFilter
	{
		protected Dictionary<Regex, Func<HttpRequest, HttpResponse, Task>> Routes = new Dictionary<Regex, Func<HttpRequest, HttpResponse, Task>>();
		Func<HttpRequest, HttpResponse, Task> DefaultRoute;

		public void SetDefaultRoute(Func<HttpRequest, HttpResponse, Task> Route)
		{
			this.DefaultRoute = Route;
		}

		public void SetDefaultRoute(IHttpFilter Route)
		{
			this.DefaultRoute = Route.FilterAsync;
		}

		public void AddRoute(string Path, IHttpFilter Route)
		{
			AddRoute(Path, Route.FilterAsync);
		}

		public void AddRoute(string Path, Func<HttpRequest, HttpResponse, Task> Route)
		{
			Routes.Add(new Regex("^" + Path + "$", RegexOptions.Compiled), Route);
		}

		async public Task RouteAsync(HttpRequest Request, HttpResponse Response)
		{
			Console.WriteLine("Request: {0}", Request.Url);
			//var UrlParts = Request.Url.Split(new[] { '?' }, 2);
			//var 
			var Path = Request.Url.Path;

			foreach (var Route in Routes)
			{
				if (Route.Key.IsMatch(Path))
				{
					await Route.Value(Request, Response);
					return;
				}
			}
			if (DefaultRoute == null)
			{
				Response.Headers["Content-Type"] = "text/html";
				Console.WriteLine("No rute found for '" + Path + "'");
				throw (new Exception("No route found!"));
			}
			else
			{
				await DefaultRoute(Request, Response);
			}
		}

		async public Task FilterAsync(HttpRequest Request, HttpResponse Response)
		{
			await RouteAsync(Request, Response);
		}
	}
}
