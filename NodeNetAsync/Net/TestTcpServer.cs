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
		async static public Task Create(Func<TcpSocket, Task> ServerClientHandle, Func<ushort, Task> ClientConnection, Action<Exception> OnError = null)
		{
			try
			{
				var TestPort = TcpServer.GetAvailablePort();
				var Server = new TcpServer(Port: TestPort, Bind: "127.0.0.1", CatchExceptions: false);
				Server.HandleClient += async (Client) =>
				{
					await ServerClientHandle(Client);
					await Client.CloseAsync();
				};
				var ListeningTask = Server.ListenAsync(1);
				{
					await ClientConnection(TestPort);
				}
				await ListeningTask;
			}
			catch (Exception Exception)
			{
				if (OnError != null) OnError(Exception);
			}
			//await Task.Delay(100);
		}
	}
}
