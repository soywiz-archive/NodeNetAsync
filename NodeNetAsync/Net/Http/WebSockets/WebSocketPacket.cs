using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Streams;

namespace NodeNetAsync.Net.Http.WebSockets
{
	public struct WebSocketPacket
	{
		/// <summary>
		/// 
		/// </summary>
		public enum OpcodeEnum : byte
		{
			ContinuationFrame = 0,
			TextFrame = 1,
			BinaryFrame = 2,
			ConnectionClose = 8,
			Ping = 9,
			Pong = 10,
		}

		/// <summary>
		/// 
		/// </summary>
		public OpcodeEnum Opcode;

		/// <summary>
		/// 
		/// </summary>
		public byte[] Payload;

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public string Text
		{
			get
			{
				return Encoding.UTF8.GetString(Payload);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public byte[] Binary
		{
			get
			{
				return Payload;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public object Data
		{
			get
			{
				switch (Opcode)
				{
					case OpcodeEnum.TextFrame: return Text;
					default: return Binary;
				}
			}
		}

		public override string ToString()
		{
			string DataString = (Data is byte[]) ? BitConverter.ToString((byte[])Data) : Data.ToString();
			return String.Format("WebSocketPacket('{0}', '{1}')", Opcode, DataString);
		}

		static async public Task<WebSocketPacket> ReadPacketFromStreamAsync(int Version, NodeBufferedStream Stream)
		{
			var Packet = default(WebSocketPacket);

			if (Version <= 0)
			{
				var PayloadBegin = await Stream.ReadBytesAsync(1);
				if (PayloadBegin[0] != 0x00) throw(new Exception("Invalid Packet"));
				Packet.Opcode = OpcodeEnum.TextFrame;
				Packet.Payload = await Stream.ReadBytesUntilByteAsync(0xFF);
			}
			else
			{
				var Header = new byte[2];
				var Size = new byte[8];
				var SizeSize = 0;
				var Mask = new byte[4];
				var Data = new MemoryStream();
				var Temp = new byte[128];
				WebSocketPacket.OpcodeEnum Opcode;
				bool IsFinal;
				bool IsMasked;
				int PayloadLength;

				do
				{
					await Stream.ReadAsync(Header, 0, 2);
					IsFinal = (((Header[0] >> 7) & 0x1) != 0);
					Opcode = (WebSocketPacket.OpcodeEnum)((Header[0] >> 0) & 0x7);
					PayloadLength = (Header[1] >> 0) & 0x7F;
					IsMasked = ((Header[1] >> 7) & 0x1) != 0;

					if (Opcode != OpcodeEnum.ContinuationFrame)
					{
						Packet.Opcode = Opcode;
					}

					// EXTENDED PayloadLength
					if (PayloadLength >= 0x7E)
					{
						if (PayloadLength == 0x7E)
						{
							SizeSize = 2;
						}
						else if (PayloadLength == 0x7F)
						{
							SizeSize = 8;
						}
						await Stream.ReadAsync(Size, 0, SizeSize);
						PayloadLength = 0;
						for (int n = 0; n < SizeSize; n++)
						{
							PayloadLength <<= 8;
							PayloadLength |= Size[n];
						}
					}

					// MASK
					if (IsMasked)
					{
						await Stream.ReadAsync(Mask, 0, 4);
					}

					// Read Payload
					await Stream.ReadAsync(Temp, 0, PayloadLength);

					// Perform unmasking
					if (IsMasked)
					{
						for (int n = 0; n < PayloadLength; n++) Temp[n] ^= Mask[n % 4];
					}

					Data.Write(Temp, 0, PayloadLength);
				} while (!IsFinal);

				Packet.Payload = Data.GetContentBytes();
			}

			return Packet;
		}
	}
}
