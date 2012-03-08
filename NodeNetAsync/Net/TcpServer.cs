using System;
using System.Collections.Generic;
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
				Exception Exception  =null;

				try
				{
					await HandleClient(Socket);
				}
				catch (Exception CatchedException)
				{
					Exception = CatchedException;
				}
				if (Exception != null)
				{
					await Console.Out.WriteLineAsync(String.Format("{0}", Exception));
				}
			}
		}

		async public Task ListenAsync()
		{
			Console.WriteLine(String.Format("Starting socket at {0}", TcpListener.LocalEndpoint));
			TcpListener.Start();
			//await Console.Out.WriteLineAsync(String.Format("Started socket at {0}", TcpListener.LocalEndpoint));
			Console.WriteLine(String.Format("Started socket at {0}", TcpListener.LocalEndpoint));

			while (true)
			{
				HandleClientInternal(new TcpSocket(await TcpListener.AcceptTcpClientAsync()));
			}
		}
	}
}
