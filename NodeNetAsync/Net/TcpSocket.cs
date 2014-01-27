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
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

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

		async static public Task<TcpSocket> CreateAndConnectAsync(string Host, int Port, int BufferSize = NodeBufferedStream.DefaultBufferSize)
		{
			var TcpSocket = new TcpSocket(new SystemTcpClient(), BufferSize);
			await TcpSocket.ConnectAsync(Host, Port);
			return TcpSocket;
		}

		public TcpSocket(int BufferSize = NodeBufferedStream.DefaultBufferSize) : base(BufferSize)
		{
			this.TcpClient = new TcpClient();
			_Init();
		}

		public bool Connected { get { return TcpClient.Connected; } }

		internal TcpSocket(SystemTcpClient TcpClient, int BufferSize = NodeBufferedStream.DefaultBufferSize)
			: base(BufferSize)
		{
			this.TcpClient = TcpClient;
			_Init();
			if (TcpClient.Connected) _Connected();
		}

		async public Task ConnectAsync(string Host, int Port)
		{
			await TcpClient.ConnectAsync(Host, Port);
			_Connected();
		}

		async public Task CloseAsync()
		{
			await FlushAsync();
			this.TcpClient.Close();
		}

		private void _Connected()
		{
			this.Stream = TcpClient.GetStream();
		}

		private void _Init()
		{
			this.TcpClient.ReceiveBufferSize = BufferSize;
			this.TcpClient.SendBufferSize = BufferSize;
			this.TcpClient.NoDelay = true;
		}

		async public Task SecureSslAsync(string Domain)
		{
			//var SslStream = new NegotiateStream(Stream);
			//await SslStream.AuthenticateAsClientAsync();
			var SslStream = new SslStream(Stream, false, (sender, certificate, chain, sslPolicyErrors) =>
			{
				//Console.WriteLine(certificate);
				return true;
			}, null);
			System.Net.WebRequest.DefaultWebProxy = null;
			await SslStream.AuthenticateAsClientAsync(Domain, null, SslProtocols.Default, false);
			//await SslStream.AuthenticateAsClientAsync(Domain);
			Stream = SslStream;
		}
	}
}
