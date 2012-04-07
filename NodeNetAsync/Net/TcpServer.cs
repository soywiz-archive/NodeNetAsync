using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net
{
	public partial class TcpServer
	{
		TcpListener TcpListener;
		public event Func<TcpSocket, Task> HandleClient;
		protected bool CatchExceptions;

		public TcpServer(ushort Port, string Bind = "127.0.0.1", bool CatchExceptions = true)
		{
			this.TcpListener = new TcpListener(IPAddress.Parse(Bind), Port);
			this.CatchExceptions = CatchExceptions;
		}

		async private Task HandleClientInternal(TcpSocket Socket)
		{
			if (HandleClient != null)
			{
				if (CatchExceptions)
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
						await Console.Error.WriteLineAsync(String.Format("{0}", YieldedException));
					}
				}
				else
				{
					await HandleClient(Socket);
				}
			}
		}

		async public Task ListenAsync(int Times = -1)
		{
			Console.Write(String.Format("Starting socket at {0}...", TcpListener.LocalEndpoint));
			TcpListener.Start();
			//await Console.Out.WriteLineAsync(String.Format("Started socket at {0}", TcpListener.LocalEndpoint));
			Console.WriteLine(String.Format("Ok"));

			while (Times != 0)
			{
				var Task = HandleClientInternal(new TcpSocket(await TcpListener.AcceptTcpClientAsync()));

				if (Core.IsRunningOnMono) Task.Start();

				//Task.RunSynchronously();
				//Task.Start(TaskScheduler.Current);
				if (Times > 0) Times--;
			}

			Console.WriteLine("Done");
		}
	}
}
