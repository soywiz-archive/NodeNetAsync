using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Db.Mysql;
using NodeNetAsync.Net.Http;
using NodeNetAsync.Net.Http.Router;
using NodeNetAsync.Utils;
using NodeNetAsync.Json;

namespace NodeNetAsync.Examples
{
	class MysqlTestProgram
	{
		static void Main(string[] args)
		{
			Core.Loop(async () =>
			{
				var Server = new HttpServer();
				var Router = new HttpRouter();

				Router.AddRoute("/test", async (Request, Response) =>
				{
					Response.Buffering = true;

					Response.Code = HttpCode.Ids.Ok;
					Response.Headers["Content-Type"] = "application/json";

					var MysqlClient = new MysqlClient("FEDORADEV", User: "test", Password: "test");
					await MysqlClient.ConnectAsync();

					foreach (var Row in await MysqlClient.QueryAsync("SELECT 1 as 'k1', 2 as 'k2', 3 * 999, 'test', 1 as 'Ok';"))
					{
						await Response.WriteChunkAsync(Row.ToJsonString());
					}

					await MysqlClient.CloseAsync();
				});

				// Default file serving
				Router.SetDefaultRoute(async (Request, Response) =>
				{
					Response.Buffering = true;

					var FilePath = Url.GetInnerFileRelativeToPath(@"C:\projects\csharp\nodenet\NodeNet\static", Request.Url);
					if (Directory.Exists(FilePath))
					{
						FilePath = FilePath + "/index.html";
					}
					Response.Headers["Content-Type"] = MimeType.GetFromPath(FilePath);

					using (var TestFile = File.OpenRead(FilePath))
					{
						await Response.CopyFromStreamASync(TestFile);
					}
				});

				Server.AddFilterLast(Router);
				await Server.ListenAsync(80, "127.0.0.1");
			});
		}
	}
}
