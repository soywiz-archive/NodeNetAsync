using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net;

namespace NodeNetAsync.Db.Mysql
{
	public class MysqlClient : IDisposable
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
		private Encoding ConnectionEncoding = new UTF8Encoding(false);
		private MysqlLanguageEnum ConnectionEncodingInternal = MysqlLanguageEnum.UTF8_UNICODE_CI;
		public byte LastPackedId;

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

		public bool IsConnected
		{
			get
			{
				if (this.TcpSocket == null) return false;
				return true;
			}
		}

		async public Task ConnectAsync()
		{
			this.TcpSocket = await TcpSocket.CreateAndConnectAsync(Host, Port);
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
			var Message = Packet.ReadStringz(ConnectionEncoding);
			/*
			Console.WriteLine("PacketNumber: {0}", Packet.PacketNumber);
			Console.WriteLine("Result:");
			Console.WriteLine("AffectedRows:{0}", AffectedRows);
			Console.WriteLine("InsertId:{0}", InsertId);
			Console.WriteLine("ServerStatus:{0}", ServerStatus);
			Console.WriteLine("WarningCount:{0}", WarningCount);
			Console.WriteLine("Message:{0}", Message);
			*/
		}

		private void HandleHandshakePacket(MysqlPacket Packet)
		{
			//Console.WriteLine(Packet.PacketNumber);
			
			//Trace.Assert(Packet.Number == 0);
			var ProtocolVersion = (MysqlProtocolVersionEnum)Packet.ReadByte();
			var ServerVersion = Packet.ReadStringz(ConnectionEncoding);
			var ThreadId = Packet.ReadUInt32();
			var Scramble0 = Packet.ReadStringzBytes();
			var ServerCapabilitiesLow = Packet.ReadUInt16();
			var ServerLanguage = Packet.ReadByte();
			var ServerStatus = Packet.ReadUInt16();
			var ServerCapabilitiesHigh = Packet.ReadUInt16();
			var PluginLength = Packet.ReadByte();
			Packet.ReadBytes(10);
			var Scramble1 = Packet.ReadStringzBytes();
			var Extra = Packet.ReadStringz(ConnectionEncoding);

			this.ScrambleBuffer = Scramble0.Concat(Scramble1).ToArray();

			var ServerCapabilities = (MysqlCapabilitiesSet)((ServerCapabilitiesLow << 0) | (ServerCapabilitiesHigh << 16));

			/*
			Console.WriteLine("PacketNumber: {0}", Packet.PacketNumber);
			Console.WriteLine(ProtocolVersion);
			Console.WriteLine(ServerVersion);
			Console.WriteLine(ServerCapabilities.ToString());
			Console.WriteLine(Scramble0);
			Console.WriteLine(Scramble1);
			Console.WriteLine(Extra);
			*/
		}

		private MysqlPacket CreateNextPacket()
		{
			//return new MysqlPacket(++LastPackedId);
			return new MysqlPacket(ConnectionEncoding, 0 + 1);
		}

		private MysqlPacket CreateNewAuthPacket()
		{
			var Token = MysqlAuth.Token((this.Password != null) ? ConnectionEncoding.GetBytes(this.Password) : null, ScrambleBuffer);
			var Packet = new MysqlPacket(ConnectionEncoding, 0 + 1);
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
			LastPackedId = PacketNumber;
			var Data = new byte[PacketLength];
			await TcpSocket.ReadAsync(Data);
			
			// Error Packet
			if (Data[0] == 0xFF)
			{
				var Packet = new MysqlPacket(ConnectionEncoding, PacketNumber, Data);
				Packet.ReadByte();
				var Errno = Packet.ReadUInt16();
				var SqlStateMarker = Packet.ReadByte();
				var SqlState = Encoding.UTF8.GetString(Packet.ReadBytes(5));
				var Message = Encoding.UTF8.GetString(Packet.ReadBytes(Packet.Available));
				throw(new MysqlException(Errno, SqlState, Message));
			}

			return new MysqlPacket(ConnectionEncoding, PacketNumber, Data);
		}

		async public Task<MysqlQueryResult> QueryAsync(string Query)
		{
			if (!IsConnected)
			{
				await ConnectAsync();
			}
			var MysqlQueryResult = new MysqlQueryResult();

			var OutPacket = new MysqlPacket(ConnectionEncoding, 0);
			OutPacket.WriteNumber(1, (uint)MysqlCommandEnum.COM_QUERY);
			OutPacket.WryteBytes(ConnectionEncoding.GetBytes(Query));
			await SendPacket(OutPacket);

			int NumberOfFields = HandleResultSetHeaderPacket(await ReadPacketAsync());
			//Console.WriteLine("Number of fields: {0}", NumberOfFields);
	
			// Read fields
			while (true)
			{
				var InPacket = await ReadPacketAsync();
				if (CheckEofPacket(InPacket)) break;
				MysqlQueryResult.Columns.Add(HandleFieldPacket(InPacket));
			}

			// Read words
			while (true)
			{
				var InPacket = await ReadPacketAsync();
				if (CheckEofPacket(InPacket)) break;
				MysqlQueryResult.Rows.Add(HandleRowDataPacket(InPacket, MysqlQueryResult.Columns));
			}

			return MysqlQueryResult;
		}

		private bool CheckEofPacket(MysqlPacket Packet)
		{
			try
			{
				if (Packet.ReadByte() == 0xFE) return true;
			}
			finally
			{
				Packet.Reset();
			}
			return false;
		}

		private int HandleResultSetHeaderPacket(MysqlPacket Packet)
		{
			// field_count: See the section "Types Of Result Packets" to see how one can distinguish the first byte of field_count from the first byte of an OK Packet, or other packet types.
			var FieldCount = Packet.ReadLengthCoded();
			// extra: For example, SHOW COLUMNS uses this to send the number of rows in the table.
			var Extra = Packet.ReadLengthCoded();
			return (int)FieldCount;
		}

		private MysqlRow HandleRowDataPacket(MysqlPacket Packet, MysqlColumns MysqlColumns)
		{
			var MysqlRow = new MysqlRow(MysqlColumns);

			for (int n = 0; n < MysqlColumns.Length; n++)
			{
				var Cell = Packet.ReadLengthCodedString();
				MysqlRow.Cells.Add(Cell);
				//Console.WriteLine(Cell);
			}

			return MysqlRow;
		}

		private MysqlField HandleFieldPacket(MysqlPacket Packet)
		{
			var MysqlField = new MysqlField();
			{
				MysqlField.Catalog = Packet.ReadLengthCodedString();
				MysqlField.Database = Packet.ReadLengthCodedString();
				MysqlField.Table = Packet.ReadLengthCodedString();
				MysqlField.OrgTable = Packet.ReadLengthCodedString();
				MysqlField.Name = Packet.ReadLengthCodedString();
				MysqlField.OrgName = Packet.ReadLengthCodedString();
				Packet.ReadByte();
				MysqlField.Charset = Packet.ReadUInt16();
				MysqlField.Length = Packet.ReadUInt32();
				MysqlField.Type = (MysqlFieldTypeEnum)Packet.ReadByte();
				MysqlField.Flags = (MysqlFieldFlagsSet)Packet.ReadUInt16();
				MysqlField.Decimals = Packet.ReadByte();
				Packet.ReadByte();
				MysqlField.Default = Packet.ReadLengthCodedBinary();

				/*
				Console.WriteLine("Catalog: '{0}'", Catalog);
				Console.WriteLine("Database: '{0}'", Database);
				Console.WriteLine("Table: '{0}'", Table);
				Console.WriteLine("OrgTable: '{0}'", OrgTable);
				Console.WriteLine("Name: '{0}'", Name);
				Console.WriteLine("OrgName: '{0}'", OrgName);
				*/
			}
			return MysqlField;
		}

		async public Task CloseAsync()
		{
			await TcpSocket.CloseAsync();
		}

		public void Dispose()
		{
			//AsyncHelpers.
			//AsyncHelpers.
			//CloseAsync().GetAwaiter().
		}
	}
}
