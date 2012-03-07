using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NodeNetAsync.Streams;
using NodeNetAsync.Utils;
using SystemTcpClient = System.Net.Sockets.TcpClient;

namespace NodeNetAsync.Net
{
	public class TcpSocket : NodeBufferedStream
	{
		//string Host;
		//int Port;
		SystemTcpClient TcpClient;

		async static public Task<TcpSocket> CreateAndConnectAsync(string Host, int Port)
		{
			var TcpSocket = new TcpSocket(new SystemTcpClient());
			await TcpSocket.ConnectAsync(Host, Port);
			return TcpSocket;
		}

		public TcpSocket()
		{
			this.TcpClient = new TcpClient();
			_Init();
		}

		async public Task ConnectAsync(string Host, int Port)
		{
			await TcpClient.ConnectAsync(Host, Port);
			_Connected();
		}

		internal TcpSocket(SystemTcpClient TcpClient)
		{
			this.TcpClient = TcpClient;
			_Init();
			if (TcpClient.Connected) _Connected();
		}

		private void _Init()
		{
			this.TcpClient.ReceiveBufferSize = 1024;
			this.TcpClient.SendBufferSize = 1024;
			this.TcpClient.NoDelay = true;
		}

		private void _Connected()
		{
			this.Stream = TcpClient.GetStream();
		}

		async public Task CloseAsync()
		{
			await FlushAsync();
			this.TcpClient.Close();
		}
	}
}
