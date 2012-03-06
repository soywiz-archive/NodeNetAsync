using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net;

namespace NodeNetAsync.Http
{
	public class HttpServer
	{
		TcpServer TcpServer;

		public event Func<HttpRequest, HttpResponse, Task> HandleRequest;

		public HttpServer(short Port = 80, string Host = "0.0.0.0")
		{
			TcpServer = new TcpServer(Port, Host);
			TcpServer.HandleClient += TcpServer_HandleClient;
		}

		async private Task TcpServer_HandleClient(TcpSocket Client)
		{
			//Console.WriteLine("HandleClient");
			// Create Request and Response
			var Request = new HttpRequest();
			var Response = new HttpResponse(Client);

			// Read Http
			var HttpLine = await Client.ReadLineAsync();

			// Read Http Headers
			while (true)
			{
				var HeaderLine = await Client.ReadLineAsync();
				if (HeaderLine.Length == 0) break;
				Request.Headers.List.Add(HttpHeader.Parse(HeaderLine));
			}

			// Handle Request
			await HandleRequest(Request, Response);

			// Finalize response
			await Response.EndAsync();
		}

		async public Task ListenAsync()
		{
			await TcpServer.ListenAsync();
			Console.WriteLine("End Http Server");
		}
	}
}
