using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net.Http;
using NodeNetAsync.Net.Http.Router;
using NodeNetAsync.Net.Https;

namespace NodeNetAsync.Examples
{
	class HttpsTestProgram
	{
		static void Main(string[] args)
		{
			Core.Loop(async () =>
			{
				var HttpRouter = new HttpRouter();
				{
					HttpRouter.AddRoute("/", async (Request, Response) =>
					{
						if (Request.Ssl)
						{
							Response.Buffering = true;
							await Response.WriteAsync("Hello World From Https!");
						}
						else
						{
							Response.Redirect("https://" + Request.Headers["HOST"] + ":443/", HttpCode.MOVED_PERMANENTLY_301);
						}
					});
				}

				var HttpsServer = new HttpsServer(
					PublicCertificateString: File.ReadAllText(@"certificate.cer"),
					PrivateKeyString: File.ReadAllText(@"private_key.key")
				);
				HttpsServer.AddFilterFirst(HttpRouter);

				var HttpServer = new HttpServer();
				HttpServer.AddFilterFirst(HttpRouter);
				
				await Task.WhenAll(
					HttpsServer.ListenAsync(443, "127.0.0.1"),
					HttpServer.ListenAsync(80, "127.0.0.1")
				);
			});
		}
	}
}
