using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net.Http;

namespace NodeNetAsync.Examples
{
	class HelloWorldProgram
	{
		static void Main(string[] args)
		{
			Core.Loop(async () =>
			{
				await HttpServer.Create(async (Request, Response) =>
				{
					Response.Buffering = true;

					Response.Code = HttpCode.Ids.Ok;
					Response.Headers["Content-Type"] = "text/plain";
					
					await Response.WriteChunkAsync("Hello World!");
				}).ListenAsync(80);
			});
		}
	}
}
