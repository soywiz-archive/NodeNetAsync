using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Streams;

namespace NodeNetAsync.Net.Http.WebSockets
{
	public class WebSocket<TType>
	{
		static internal int LastUniqueId = 0;
		public int UniqueId;
		public HttpRequest Request;
		public HttpResponse Response;
		public TcpSocket Socket;
		public Encoding DefaultEncoding = Encoding.UTF8;
		public TType Tag;

		public int Version
		{
			get
			{
				return this.Response.WebSocketVersion;
			}
		}

		internal WebSocket(HttpRequest Request, HttpResponse Response)
		{
			this.Request = Request;
			this.Response = Response;
			this.Socket = Response.Socket;
			this.UniqueId = LastUniqueId++;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		/// <see cref="5.2.  Base Framing Protocol"/>
		async public Task<WebSocketPacket> ReadPacketAsync()
		{
			return await WebSocketPacket.ReadPacketFromStreamAsync(Version, Socket);
		}

		async public Task WritePacketAsync(string Text, Encoding Encoding = null)
		{
			if (Encoding == null) Encoding = DefaultEncoding;
			await WritePacketAsync(Encoding.GetBytes(Text));
		}

		async public Task WritePacketAsync(byte[] Data, int Offset = 0, int Count = -1)
		{
			if (Count == -1) Count = Data.Length;

			var MemoryStream = new MemoryStream();

			if (Version <= 0)
			{
				MemoryStream.WriteByte(0x00);
				MemoryStream.Write(Data, Offset, Count);
				MemoryStream.WriteByte(0xFF);

				await Socket.WriteAsync(MemoryStream.GetBuffer(), 0, (int)MemoryStream.Length);
			}
			else
			{
				do
				{
					// @TODO: Implement longer packets
					int ChunkSize = Math.Min(Count, 0x7D); // 0x7E and 0x7F are reserved for length extension
					bool IsFinal = (ChunkSize == Count);
					bool IsMasked = false;

					MemoryStream.WriteByte((byte)((byte)(WebSocketPacket.OpcodeEnum.TextFrame) | (IsFinal ? 0x80 : 0x00)));
					MemoryStream.WriteByte((byte)(ChunkSize | (IsMasked ? 0x80 : 0x00)));
					MemoryStream.Write(Data, Offset, ChunkSize);

					Offset += ChunkSize;
					Count -= ChunkSize;
				} while (Count > 0);

				await Socket.WriteAsync(MemoryStream.GetBuffer(), 0, (int)MemoryStream.Length);
			}
		}
	}
}
