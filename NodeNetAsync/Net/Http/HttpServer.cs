using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Net.Http
{
	public class HttpServer
	{
		TcpServer TcpServer;

		public event Func<HttpRequest, HttpResponse, Task> HandleRequest;
		static Encoding HeaderEncoding = Encoding.GetEncoding("ISO-8859-1");

		static public HttpServer Create(Func<HttpRequest, HttpResponse, Task> HandleRequest)
		{
			return new HttpServer(HandleRequest);
		}

		public HttpServer(Func<HttpRequest, HttpResponse, Task> HandleRequest = null)
		{
			this.HandleRequest = HandleRequest;
		}

		async protected virtual Task ReadHeadersAsync(TcpSocket Client, HttpRequest Request, HttpResponse Response)
		{
			// Read Http
			var HttpLine = await Client.ReadLineAsync(HeaderEncoding);
			if (Debugger.IsAttached)
			{
				Console.WriteLine("Connection({0}) : {1}", Request.ConnectionId, HttpLine);
			}

			if (HttpLine == "") throw(new IOException(""));

			/*
			if (HttpLine == "")
			{
				Console.WriteLine("  Empty!");
				HttpLine = await Client.ReadLineAsync(HeaderEncoding);
				Console.WriteLine("  Connection({0}) : {1}", Request.ConnectionId, HttpLine);
			}
			*/

			var HttpParts = HttpLine.Split(new[] { ' ' }, 3);
			if (HttpParts.Length < 3)
			{
				throw (new InvalidOperationException(String.Format("Invalid HTTP Request Connection({0}) : {1}", Request.ConnectionId, HttpLine)));
			}
			Request.Method = HttpParts[0].ToUpperInvariant();
			Request.Url = HttpParts[1];
			Request.HttpVersion = HttpParts[2];

			// Read Http Headers
			while (true)
			{
				var HeaderLine = await Client.ReadLineAsync(HeaderEncoding);
				if (HeaderLine.Length == 0) break;
				Request.Headers.Add(HttpHeader.Parse(HeaderLine));
			}
		}

		async protected virtual Task InitializeConnectionAsync(TcpSocket Client)
		{
			await Task.Yield();
		}

		static int LastConnectionId = 0;

		async private Task TcpServer_HandleClient(TcpSocket Client)
		{
			Exception YieldedException = null;

			//Console.WriteLine("HandleClient");
			// Create Request and Response
			int ConnectionId = LastConnectionId++;

			await InitializeConnectionAsync(Client);

			bool KeepAlive = true;
			//bool KeepAlive = false;
			try
			{
				do
				{
					var Request = new HttpRequest();
					var Response = new HttpResponse(Client);
					Request.ConnectionId = ConnectionId;

					await ReadHeadersAsync(Client, Request, Response);

					Response.Headers["Content-Type"] = "text/html";

					switch (Request.Headers["Connection"].ToLowerInvariant())
					{
						case "keep-alive":
							Response.Headers["Connection"] = "keep-alive";
							break;
						default:
						case "close":
							Response.Headers["Connection"] = "close";
							KeepAlive = false;
							break;
					}

					//Console.WriteLine("aaaaaaaa");

					try
					{
						// Handle Request
						foreach (var Filter in FilterList) await Filter.Filter(Request, Response);

						if (HandleRequest != null) await HandleRequest(Request, Response);
					}
					catch (HttpException HttpException)
					{
						Response.SetHttpCode(HttpException.HttpCode);
					}
					catch (IOException)
					{
					}
					catch (Exception Exception)
					{
						YieldedException = Exception;
					}

					if (YieldedException != null)
					{
						if (Debugger.IsAttached)
						{
							Console.WriteLine("YIELD!!!!!!!!!!!!!!!!!! : " + YieldedException.ToString());
						}
						await Response.WriteAsync("--><pre>" + Html.Quote(YieldedException.ToString()) + "</pre>");
						YieldedException = null;
					}

					// Finalize response
					await Response.EndAsync();
				} while (KeepAlive);
			}
			catch (IOException)
			{
			}
			catch (Exception Exception)
			{
				YieldedException = Exception;
			}

			if (YieldedException != null)
			{
				//Console.WriteLine("YIELD!!!!!!!!!!!!!!!!!! : " + YieldedException.ToString());
				if (Debugger.IsAttached)
				{
					Console.WriteLine("YIELD!!!!!!!!!!!!!!!!!! : " + YieldedException.ToString());
				}
				YieldedException = null;
			}

			await Client.CloseAsync();
		}

		async public virtual Task ListenAsync(ushort Port = 80, string Host = "0.0.0.0")
		{
			this.TcpServer = new TcpServer(Port, Host);
			this.TcpServer.HandleClient += TcpServer_HandleClient;
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
