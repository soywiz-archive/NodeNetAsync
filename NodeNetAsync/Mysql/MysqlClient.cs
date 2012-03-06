using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net;

namespace NodeNetAsync.Mysql
{
	public class MysqlClient
	{
		private string Host;
		private int Port;
		private string User;
		private string Password;
		private string Database;
		private bool Debug;
		private TcpSocket TcpSocket;
		private int MaxPacketSize;
		private byte[] ScrambleBuffer;
		private Encoding ConnectionEncoding = Encoding.UTF8;
		private MysqlLanguageEnum ConnectionEncodingInternal = MysqlLanguageEnum.UTF8_UNICODE_CI;

		public MysqlClient(string Host = "localhost", int Port = 3306, string User = null, string Password = null, string Database = null, bool Debug = false, int MaxPacketSize = 0x01000000)
		{
			this.Host = Host;
			this.Port = Port;
			this.User = User;
			this.Password = Password;
			this.Database = Database;
			this.Debug = Debug;
			this.MaxPacketSize = MaxPacketSize;
		}

		async public Task ConnectAsync()
		{
			this.TcpSocket = new TcpSocket(Host, Port);
			HandleHandshakePacket(await ReadPacketAsync());
			await SendPacket(CreateNewAuthPacket());
			HandleResultPacket(await ReadPacketAsync());
		}

		private void HandleResultPacket(MysqlPacket Packet)
		{
			var FieldCount = Packet.ReadByte();
			var AffectedRows = Packet.ReadLengthCoded();
			var InsertId = Packet.ReadLengthCoded();
			var ServerStatus = Packet.ReadUInt16();
			var WarningCount = Packet.ReadUInt16();
			var Message = Packet.ReadStringz();
			Console.WriteLine("Result:");
			Console.WriteLine("AffectedRows:{0}", AffectedRows);
			Console.WriteLine("InsertId:{0}", InsertId);
			Console.WriteLine("ServerStatus:{0}", ServerStatus);
			Console.WriteLine("WarningCount:{0}", WarningCount);
			Console.WriteLine("Message:{0}", Message);
		}

		private void HandleHandshakePacket(MysqlPacket Packet)
		{
			Console.WriteLine(Packet.PacketNumber);
			//Trace.Assert(Packet.Number == 0);
			var ProtocolVersion = (MysqlProtocolVersionEnum)Packet.ReadByte();
			var ServerVersion = Packet.ReadStringz();
			var ThreadId = Packet.ReadUInt32();
			var Scramble0 = Packet.ReadStringzBytes();
			var ServerCapabilitiesLow = Packet.ReadUInt16();
			var ServerLanguage = Packet.ReadByte();
			var ServerStatus = Packet.ReadUInt16();
			var ServerCapabilitiesHigh = Packet.ReadUInt16();
			var PluginLength = Packet.ReadByte();
			Packet.ReadBytes(10);
			var Scramble1 = Packet.ReadStringzBytes();
			var Extra = Packet.ReadStringz();

			this.ScrambleBuffer = Scramble0.Concat(Scramble1).ToArray();

			var ServerCapabilities = (MysqlCapabilitiesSet)((ServerCapabilitiesLow << 0) | (ServerCapabilitiesHigh << 16));

			Console.WriteLine(ProtocolVersion);
			Console.WriteLine(ServerVersion);
			Console.WriteLine(ServerCapabilities.ToString());
			Console.WriteLine(Scramble0);
			Console.WriteLine(Scramble1);
			Console.WriteLine(Extra);
		}

		private MysqlPacket CreateNewAuthPacket()
		{
			var Token = MysqlAuth.Token((this.Password != null) ? ConnectionEncoding.GetBytes(this.Password) : null, ScrambleBuffer);
			var Packet = new MysqlPacket(0 + 1);
			Packet.WriteNumber(4, (uint)MysqlCapabilitiesSet.DEFAULT);
			Packet.WriteNumber(4, (uint)this.MaxPacketSize);
			Packet.WriteNumber(1, (uint)ConnectionEncodingInternal);
			Packet.WriteFiller(23);
			Packet.WriteNullTerminated(this.User, ConnectionEncoding);
			Packet.WriteLengthCodedString(Token);
			Packet.WriteNullTerminated(this.Database, ConnectionEncoding);
			return Packet;
		}

		async private Task SendPacket(MysqlPacket Packet)
		{
			await Packet.SendToAsync(this.TcpSocket);
		}

		async private Task<MysqlPacket> ReadPacketAsync()
		{
			var HeaderData = new byte[4];
			await TcpSocket.ReadAsync(HeaderData, 0, HeaderData.Length);
			var PacketLength = (HeaderData[0] << 0) | (HeaderData[1] << 8) | (HeaderData[2] << 16);
			var PacketNumber = HeaderData[3];
			var Data = new byte[PacketLength];
			await TcpSocket.ReadAsync(Data);
			return new MysqlPacket(PacketNumber, Data);
		}
	}
}
