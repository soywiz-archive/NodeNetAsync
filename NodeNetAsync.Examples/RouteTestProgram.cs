using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net.Http;
using NodeNetAsync.Net.Http.Router;
using NodeNetAsync.Net.Http.Static;
using NodeNetAsync.Net.Http.WebSockets;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Examples
{
	public class RouteTestProgram
	{
		static public void Main(string[] Args)
		{
			Core.Loop(async () =>
			{
				var Server = new HttpServer();
				var Router = new HttpRouter();

				Router.AddRoute("/test", async (Request, Response) =>
				{
					await Response.WriteAsync("Test!");
				});

				// Default file serving
				Router.SetDefaultRoute(new HttpStaticFileServer(Path: @"C:\projects\csharp\nodenet\NodeNet\static", Cache: true));

				Server.AddFilterLast(Router);
				await Server.ListenAsync(80, "127.0.0.1");
			});
		}
	}
}
