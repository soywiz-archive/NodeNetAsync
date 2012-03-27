using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net;
using NodeNetAsync.Streams;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Net.Http
{
	public class HttpServer
	{
		TcpServer TcpServer;

		public event Func<HttpRequest, HttpResponse, Task> HandleRequest;
		static Encoding HeaderEncoding = Encoding.GetEncoding("ISO-8859-1");
		public ushort Port { get; protected set; }
		public string Host { get; protected set; }
		public int MaxKeepAliveRequests = 100;

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
			//if (Debugger.IsAttached) Console.WriteLine("REQUEST: Connection({0}) : '{1}'", Request.ConnectionId, HttpLine);

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

			if (!Request.HttpVersion.StartsWith("HTTP/1."))
			{
				throw (new InvalidOperationException(String.Format("Invalid HTTP Request Connection({0}) : {1}", Request.ConnectionId, HttpLine)));
			}

			// Read Http Headers
			while (true)
			{
				var HeaderLine = await Client.ReadLineAsync(HeaderEncoding);

				//if (Debugger.IsAttached) Console.WriteLine("    HEADER: Connection({0}) : '{1}'", Request.ConnectionId, HeaderLine);

				if (HeaderLine.Length == 0) break;
				Request.Headers.Add(HttpHeader.Parse(HeaderLine));
				if (Request.Headers.Count > 100) throw (new SequenceTooLongException());
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
			int KeepAliveCount = 0;
			//bool KeepAlive = false;
			try
			{
				do
				{
					var Request = new HttpRequest(this.Port, ConnectionId, KeepAliveCount++);
					var Response = new HttpResponse(Client);

					try
					{
						try
						{
							await ReadHeadersAsync(Client, Request, Response);
						}
						catch (SequenceTooLongException)
						{
							KeepAlive = false;
							//throw(new HttpException(HttpCode.REQUEST_URI_TOO_LONG_412));
							throw (new HttpException(HttpCode.REQUEST_HEADER_FIELDS_TOO_LARGE_431));
						}
						catch (Exception Exception)
						{
							KeepAlive = false;
							throw Exception;
						}

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

						// Reached maximum KeepAlive Requests.
						if (KeepAliveCount >= MaxKeepAliveRequests)
						{
							Response.Headers["Connection"] = "close";
							KeepAlive = false;
						}

						Request.Content = new byte[0];
						var ContentLengthString = Request.Headers["Content-Length"];
						if (ContentLengthString != "" && ContentLengthString != null)
						{
							int ContentLength = 0;
							if (!int.TryParse(ContentLengthString, out ContentLength))
							{
								throw(new HttpException(HttpCode.BAD_REQUEST_400));
							}
							if (ContentLength > 128 * 1024)
							{
								throw (new HttpException(HttpCode.BAD_REQUEST_400));
							}
							Request.Content = await Client.ReadBytesAsync(ContentLength);
						}

						// Apply Pre Request filters
						foreach (var Filter in FilterList)
						{
							await Filter.FilterAsync(Request, Response);
						}

						// Main HandleRequest
						if (HandleRequest != null)
						{
							await HandleRequest(Request, Response);
						}
					}
					catch (HttpException HttpException)
					{
						Response.Buffering = true;
						Response.ChunkedTransferEncoding = true;
						//Response.Buffering = false;
						//Response.ChunkedTransferEncoding = true;
						Response.SetHttpCode(HttpException.HttpCode);
						YieldedException = HttpException;
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
						if (YieldedException is HttpException) {
							var HttpException = YieldedException as HttpException;
							if ((int)HttpException.HttpCode >= 400)
							{
								await Response.WriteAsync("<h1>" + HttpException.HttpCode + "</h1>");
							}
						}
						else
						{
							if (Debugger.IsAttached)
							{
								await Response.WriteAsync("--><pre>" + Html.Quote(YieldedException.ToString()) + "</pre>");
							}

							//await Console.Out.WriteLineAsync("Exception : " + YieldedException.ToString());
							Console.WriteLine("Exception : " + YieldedException.ToString());
						}
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
			this.Port = Port;
			this.Host = Host;
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
