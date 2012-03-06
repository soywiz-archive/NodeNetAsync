using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Net
{
	public class TcpSocket
	{
		TcpClient TcpClient;
		NetworkStream Stream;
		StreamReader StreamReader;
		StreamWriter StreamWriter;

		public TcpSocket(string Host, int Port)
		{
			this.TcpClient = new TcpClient(Host, Port);
			_Init();
		}

		internal TcpSocket(TcpClient TcpClient)
		{
			this.TcpClient = TcpClient;
			_Init();
		}

		private void _Init()
		{
			this.Stream = TcpClient.GetStream();
			this.StreamReader = new StreamReader(Stream);
			this.StreamWriter = new StreamWriter(Stream);
			this.StreamWriter.AutoFlush = true;
		}

		async public Task<string> ReadLineAsync()
		{
			return await this.StreamReader.ReadLineAsync();
		}

		async public Task FlushAsync()
		{
			await this.Stream.FlushAsync();
		}

		async public Task CloseAsync()
		{
			//await this.StreamWriter.FlushAsync();
			await FlushAsync();
			this.TcpClient.Close();
		}

		async public Task WriteAsync(string Text, Encoding Encoding)
		{
			//Console.WriteLine(Text);
			using (var Writer = new StreamWriter(Stream, Encoding, 1024, true))
			{
				Writer.NewLine = "\r\n";
				Writer.AutoFlush = true;
				await Writer.WriteAsync(Text);
			}
		}

		async public Task<int> ReadAsync(byte[] Buffer, int Offset = 0, int Count = -1)
		{
			if (Count == -1) Count = Buffer.Length;
			return await Stream.ReadAsync(Buffer, Offset, Count);
		}

		async public Task WriteAsync(byte[] Buffer, int Offset = 0, int Count = -1)
		{
			if (Count == -1) Count = Buffer.Length;
			await Stream.WriteAsync(Buffer, Offset, Count);
		}
	}
}
