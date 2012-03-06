using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net;

namespace NodeNetAsync.Mysql
{
	public partial class MysqlPacket
	{
		public byte PacketNumber;
		private Encoding Encoding;
		private MemoryStream Stream;
		private StreamReader StreamReader;

		public MysqlPacket(Encoding Encoding, byte PacketNumber, byte[] ByteData = null)
		{
			this.Encoding = Encoding;
			this.PacketNumber = PacketNumber;
			this.Stream = (ByteData != null) ? new MemoryStream(ByteData) : new MemoryStream();
			this.StreamReader = new StreamReader(Stream);
		}

		public int Length { get { return (int)Stream.Length; } }
		public int Available { get { return (int)(Stream.Length - Stream.Position); } }

		public MemoryStream ReadStringzMemoryStream()
		{
			var Buffer = new MemoryStream();
			while (Stream.Position < Stream.Length)
			{
				int c = Stream.ReadByte();
				if (c == -1) break;
				if (c == 0) break;
				Buffer.WriteByte((byte)c);
			}
			Buffer.Position = 0;
			return Buffer;
		}

		byte[] GetPacketBytes()
		{
			var Data = new byte[Stream.Length];
			var Buffer = this.Stream.GetBuffer();
			Array.Copy(Buffer, 0, Data, 0, Data.Length);
			return Data;
		}

		public byte[] ReadLengthCodedStringBytes()
		{
			var Size = ReadLengthCoded();
			if (Size == null) return null;
			return ReadBytes((int)Size);
		}

		public String ReadLengthCodedString()
		{
			var Bytes = ReadLengthCodedStringBytes();
			if (Bytes == null) return null;
			return Encoding.GetString(Bytes);
		}

		public byte[] ReadLengthCodedBinary()
		{
			return ReadLengthCodedStringBytes();
		}

		public void Reset()
		{
			Stream.Position = 0;
		}
	}

	public partial class MysqlPacket
	{
		public byte[] ReadStringzBytes()
		{
			var Buffer = ReadStringzMemoryStream();
			var Out = new byte[Buffer.Length];
			Buffer.Read(Out, 0, Out.Length);
			return Out;
		}

		public string ReadStringz()
		{
			var Buffer = ReadStringzMemoryStream();
			return Encoding.UTF8.GetString(Buffer.GetBuffer(), 0, (int)Buffer.Length);
		}

		public byte ReadByte()
		{
			return (byte)Stream.ReadByte();
		}

		public byte[] ReadBytes(int Count)
		{
			var Bytes = new byte[Count];
			Stream.Read(Bytes, 0, Count);
			return Bytes;
		}

		public ushort ReadUInt16()
		{
			var V0 = ReadByte();
			var V1 = ReadByte();
			return (ushort)((V0 << 0) | (V1 << 8));
		}

		public uint ReadUInt32()
		{
			var V0 = ReadUInt16();
			var V1 = ReadUInt16();
			return (uint)((V0 << 0) | (V1 << 16));
		}

		public ulong ReadUInt64()
		{
			var V0 = ReadUInt32();
			var V1 = ReadUInt32();
			return (ulong)((V0 << 0) | (V1 << 32));
		}

		public ulong? ReadLengthCoded()
		{
			int ReadCount = 0;
			var First = ReadByte();
			if (First <= 250)
			{
				return First;
			}
			else
			{
				switch (First)
				{
					case 251: return null;
					case 252: ReadCount = 2; break;
					case 253: ReadCount = 3; break;
					case 254: ReadCount = 8; break;
				}
			}
			ulong Value = 0;
			for (int n = 0; n < ReadCount; n++)
			{
				Value |= ((ulong)ReadByte() << (8 * n));
			}
			return Value;
		}
	}

	public partial class MysqlPacket
	{
		async public Task SendToAsync(TcpSocket Client)
		{
			Stream.Position = 0;
			var PacketSize = (uint)Stream.Length;
			byte[] Header = new byte[4];
			Header[0] = (byte)(PacketSize >> 0);
			Header[1] = (byte)(PacketSize >> 8);
			Header[2] = (byte)(PacketSize >> 16);
			Header[3] = this.PacketNumber;
			await Client.WriteAsync(Header);
			await Client.WriteAsync(GetPacketBytes());
			await Client.FlushAsync();
		}

		public void WriteNumber(int BytesCount, uint Value)
		{
			for (int n = 0; n < BytesCount; n++)
			{
				this.Stream.WriteByte((byte)Value);
				Value >>= 8;
			}
		}

		public void WriteFiller(int Count)
		{
			this.Stream.Write(new byte[Count], 0, Count);
		}

		public void WriteNullTerminated(byte[] Data)
		{
			if (Data != null) this.Stream.Write(Data, 0, Data.Length);
			this.Stream.WriteByte(0);
		}

		public void WriteNullTerminated(string String, Encoding Encoding)
		{
			WriteNullTerminated((String != null) ? Encoding.GetBytes(String) : null);
		}

		public void WriteLengthCodedInt(ulong Value)
		{
			int Count = 0;

			if (Value <= 250)
			{
				Count = 1;
			}
			// 16 bits
			else if (Value <= 0xffff)
			{
				this.Stream.WriteByte(252);
				Count = 2;
			}
			// 24 bits
			else if (Value <= 0xffffff)
			{
				this.Stream.WriteByte(253);
				Count = 3;
			}
			// 64 bits
			else
			{
				this.Stream.WriteByte(254);
				Count = 8;
			}

			while (Count-- > 0)
			{
				this.Stream.WriteByte((byte)(Value));
				Value >>= 8;
			}
		}

		public void WriteLengthCodedString(byte[] Value)
		{
			WriteLengthCodedInt((uint)Value.Length);
			this.Stream.Write(Value, 0, Value.Length);
		}

		public void WryteBytes(byte[] Value, int Offset = 0, int Length = -1)
		{
			if (Length == -1) Length = Value.Length;
			this.Stream.Write(Value, Offset, Length);
		}
	}
}
