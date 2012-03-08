using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Streams
{
	public class NodeBufferedStream
	{
		//protected const int DefaultBufferSize = 32;
		protected const int DefaultBufferSize = 128;
		//protected const int DefaultBufferSize = 256;
		//protected const int DefaultBufferSize = 1024;

		protected Stream Stream;
		protected byte[] TempBuffer = new byte[DefaultBufferSize];
		protected ByteRingBuffer RingBuffer = new ByteRingBuffer(DefaultBufferSize);
		public Encoding DefaultEncoding = Encoding.UTF8;

		protected NodeBufferedStream()
		{
		}

		public NodeBufferedStream(Stream Stream)
		{
			this.Stream = Stream;
		}

		private bool IsForSureDataAvailable
		{
			get
			{
				if (Stream is NetworkStream) return (Stream as NetworkStream).DataAvailable;
				return false;
			}
		}

		async private Task FillBuffer(int MinimumSize)
		{
			MinimumSize = Math.Min(MinimumSize, RingBuffer.Size);

			while (
				(
					//IsForSureDataAvailable ||
					(RingBuffer.AvailableForRead < MinimumSize)
				)
				&& (RingBuffer.AvailableForWrite > 0)
			)
			{
				int ToRead = RingBuffer.AvailableForWrite;

				int Readed = await Stream.ReadAsync(TempBuffer, 0, ToRead);
				if (Readed <= 0) throw (new IOException());
				RingBuffer.Write(TempBuffer, 0, Readed);
			}
		}

		async public Task FlushAsync()
		{
			await this.Stream.FlushAsync();
		}

		async public Task<string> ReadLineAsync(Encoding Encoding = null)
		{
			if (Encoding == null) Encoding = this.DefaultEncoding;
			var String = Encoding.GetString(await ReadLineAsByteArrayAsync());
			//Console.WriteLine(String);
			return String;
		}

		async public Task<byte[]> ReadLineAsByteArrayAsync()
		{
			var Data = (await ReadLineAsMemoryStreamAsync());
			var Return = new byte[Data.Length];
			Array.Copy(Data.GetBuffer(), 0, Return, 0, Return.Length);
			return Return;
		}

		async public Task<MemoryStream> ReadLineAsMemoryStreamAsync()
		{
			var Return = new MemoryStream();
			bool Found = false;
			do
			{
				await FillBuffer(1);
				int Readed = RingBuffer.Peek(TempBuffer, 0, RingBuffer.AvailableForRead);
				//Console.WriteLine(Readed);
				for (int n = 0; n < Readed; n++)
				{
					byte c = TempBuffer[n];
					if (c == '\r' || c == '\n')
					{
						Readed = n;
						Found = true;
						break;
					}
				}
				//Console.WriteLine(Readed);
				if (Readed > 0)
				{
					Return.Write(TempBuffer, 0, Readed);
					RingBuffer.Skip(Readed);
				}
			} while (!Found);

			await FillBuffer(1);
			if (RingBuffer.Peek(TempBuffer, 0, 2) >= 2)
			{
				if (TempBuffer[0] == '\r' && TempBuffer[1] == '\n')
				{
					//Return.Write(TempBuffer, 0, 2);
					RingBuffer.Skip(2);
					return Return;
				}
			}
			//Return.Write(TempBuffer, 0, 1);
			RingBuffer.Skip(1);
			return Return;
		}

		async public Task<int> ReadAsync(byte[] Buffer, int Offset = 0, int Count = -1)
		{
			if (Count < 0) Count = Buffer.Length;
			int Readed = 0;
			while (Count > 0)
			{
				await FillBuffer(1);
				int ToReadStep = Math.Min(Count, RingBuffer.AvailableForRead);
				int ReadedStep = RingBuffer.Read(Buffer, Offset, ToReadStep);
				Offset += ReadedStep;
				Readed += ReadedStep;
				Count -= ReadedStep;
			}
			return Readed;
		}

		async public Task WriteAsync(byte[] Buffer, int Offset = 0, int Count = -1)
		{
			if (Count == -1) Count = Buffer.Length;
			await Stream.WriteAsync(Buffer, Offset, Count);
		}

		async public Task WriteAsync(string Text, Encoding Encoding = null)
		{
			if (Encoding == null) Encoding = this.DefaultEncoding;
			await WriteAsync(Encoding.GetBytes(Text));
		}

		async public Task SkipBytesAsync(int Count)
		{
			await ReadAsync(null, 0, Count);
		}

		async public Task<byte[]> ReadBytesAsync(int Count)
		{
			var Data = new byte[Count];
			await ReadAsync(Data, 0, Count);
			return Data;
		}

		async public Task<string> ReadStringAsync(int Count, Encoding Encoding = null)
		{
			if (Encoding == null) Encoding = this.DefaultEncoding;
			return Encoding.GetString(await ReadBytesAsync(Count));
		}
	}
}
