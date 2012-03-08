using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net;

namespace NodeNetAsync.Net
{
	public class TestTcpServer
	{
		async static public Task Create(Func<TcpSocket, Task> Server, Func<ushort, Task> Client)
		{
			var TestPort = TcpServer.GetAvailablePort();
			var TcpListener = new TcpServer(Port: TestPort, Bind: "127.0.0.1", CatchExceptions: false);
			TcpListener.HandleClient += async (TcpSocket) =>
			{
				await Server(TcpSocket);
				await TcpSocket.CloseAsync();
			};
			var ListeningTask = TcpListener.ListenAsync(1);
			{
				await Client(TestPort);
			}
			await ListeningTask;
		}
	}
}
