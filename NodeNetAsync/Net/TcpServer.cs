using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net
{
	public class TcpServer
	{
		TcpListener TcpListener;
		public event Func<TcpSocket, Task> HandleClient;

		public TcpServer(short Port, string Bind = "0.0.0.0")
		{
			this.TcpListener = new TcpListener(IPAddress.Parse(Bind), Port);
		}

		async private Task HandleClientInternal(TcpSocket Socket)
		{
			if (HandleClient != null)
			{
				Exception YieldedException = null;

				try
				{
					await HandleClient(Socket);
				}
				catch (IOException)
				{
				}
				catch (Exception CatchedException)
				{
					YieldedException = CatchedException;
				}

				if (YieldedException != null)
				{
					await Console.Out.WriteLineAsync(String.Format("{0}", YieldedException));
				}
			}
		}

		async public Task ListenAsync()
		{
			Console.Write(String.Format("Starting socket at {0}...", TcpListener.LocalEndpoint));
			TcpListener.Start();
			//await Console.Out.WriteLineAsync(String.Format("Started socket at {0}", TcpListener.LocalEndpoint));
			Console.WriteLine(String.Format("Ok"));

			while (true)
			{
				HandleClientInternal(new TcpSocket(await TcpListener.AcceptTcpClientAsync()));
			}
		}
	}
}
