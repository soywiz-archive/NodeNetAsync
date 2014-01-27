using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net.Http;
using NodeNetAsync.Net;
using NodeNetAsync.Net.Xmpp;

namespace NodeNetAsync.Examples
{
	class HelloWorldProgram
	{
		static public void Main(string[] args)
		{
			Core.Loop(async () =>
			{
				await HttpServer.Create(async (Request, Response) =>
				{
					Response.Buffering = true;

					Response.SetHttpCode(HttpCode.OK_200);
					Response.Headers["Content-Type"] = "text/plain";
					
					await Response.WriteAsync("Hello World!");
				}).ListenAsync(8081);
			});
		}
	}
}
