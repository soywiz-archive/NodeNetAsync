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

		/*
		public event Action Connected;
		public event Action Data;
		public event Action Disconnected;
		*/

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

		/*
		private void _Read()
		{
			var BufferSize = 1024;
			var Buffer = new byte[BufferSize];
			this.Stream.BeginRead(Buffer, 0, BufferSize, (AsyncResult) =>
			{
				var Readed = this.Stream.EndRead(AsyncResult);
				new ArraySegment<byte>(Buffer, 0, Readed);
				_Read();
			}, null);
		}
		*/

		private void _Connected()
		{
			this.Stream = TcpClient.GetStream();
			//if (Connected != null) Connected();
			//this.Stream.BeginRead(
			//_Read();
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

		async public Task CloseAsync()
		{
			await FlushAsync();
			this.TcpClient.Close();
		}
	}
}
