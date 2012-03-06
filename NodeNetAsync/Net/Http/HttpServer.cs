using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net;

namespace NodeNetAsync.Net.Http
{
	public class HttpServer
	{
		TcpServer TcpServer;

		public event Func<HttpRequest, HttpResponse, Task> HandleRequest;
		static Encoding HeaderEncoding = Encoding.GetEncoding("ISO-8859-1");

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
			var HttpLine = await Client.ReadLineAsync(HeaderEncoding);
			var HttpParts = HttpLine.Split(new[] { ' ' }, 3);
			if (HttpParts.Length < 3) throw(new InvalidOperationException("Invalid HTTP Request"));
			Request.Method = HttpParts[0].ToUpperInvariant();
			Request.Url = HttpParts[1];
			Request.HttpVersion = HttpParts[2];

			// Read Http Headers
			while (true)
			{
				var HeaderLine = await Client.ReadLineAsync(HeaderEncoding);
				if (HeaderLine.Length == 0) break;
				Request.Headers.List.Add(HttpHeader.Parse(HeaderLine));
			}

			Exception YieldedException = null;

			Response.Headers["Content-Type"] = "text/html";
			Response.Headers["Transfer-Encoding"] = "chunked";
			Response.Headers["Connection"] = "close";

			try
			{
				// Handle Request
				foreach (var Filter in FilterList) await Filter.Filter(Request, Response);

				if (HandleRequest != null) await HandleRequest(Request, Response);
			}
			catch (Exception Exception)
			{
				YieldedException = Exception;
			}

			if (YieldedException != null)
			{
				//Console.WriteLine("YIELD!!!!!!!!!!!!!!!!!! : " + YieldedException.ToString());
				await Response.WriteChunkAsync(YieldedException.ToString());
			}

			// Finalize response
			await Response.EndAsync();
		}

		async public Task ListenAsync()
		{
			await TcpServer.ListenAsync();
			Console.WriteLine("End Http Server");
		}

		List<IHttpFilter> FilterList = new List<IHttpFilter>();

		public void AddFilterFirst(IHttpFilter Filter)
		{
			FilterList.Insert(0, Filter);
		}

		public void AddFilterLast(IHttpFilter Filter)
		{
			FilterList.Add(Filter);
		}
	}
}
