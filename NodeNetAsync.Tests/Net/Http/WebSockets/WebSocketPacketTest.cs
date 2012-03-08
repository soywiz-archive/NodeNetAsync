using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NodeNetAsync.Net.Http.WebSockets;
using NodeNetAsync.Streams;

namespace NodeNetAsync.Tests.Net.Http.WebSockets
{
	/// <summary>
	/// 5.7. Examples
	/// </summary>
	/// <see cref="http://tools.ietf.org/html/rfc6455"/>
	[TestClass]
	public class WebSocketPacketTest
	{
		static private NodeBufferedStream GenerateBufferedStreamFromBytes(byte[] Bytes)
		{
			var Data = new MemoryStream();
			var Writer = new BinaryWriter(Data);
			Writer.Write(Bytes);
			Data.Position = 0;
			return new NodeBufferedStream(Data);
		}

		[TestMethod]
		async public Task TestSingleFrameUnmaskedTextMessage()
		{
			var Stream = GenerateBufferedStreamFromBytes(
				new byte[] { 0x81, 0x05, 0x48, 0x65, 0x6c, 0x6c, 0x6f } // Contains "Hello"
			);
			var Packet = await WebSocketPacket.ReadPacketFromStreamAsync(Version: 13, Stream: Stream);
			Assert.AreEqual("WebSocketPacket('TextFrame', 'Hello')", Packet.ToString());
		}

		[TestMethod]
		async public Task TestSingleFrameMaskedTextMessage()
		{
			var Stream = GenerateBufferedStreamFromBytes(
				new byte[] { 0x81, 0x85, 0x37, 0xfa, 0x21, 0x3d, 0x7f, 0x9f, 0x4d, 0x51, 0x58 } // Contains "Hello"
			);
			var Packet = await WebSocketPacket.ReadPacketFromStreamAsync(Version: 13, Stream: Stream);
			Assert.AreEqual("WebSocketPacket('TextFrame', 'Hello')", Packet.ToString());
		}

		[TestMethod]
		async public Task TestFragmentedUnmaskedTextMessage()
		{
			var Stream = GenerateBufferedStreamFromBytes(
				new byte[] { 0x01, 0x03, 0x48, 0x65, 0x6c } // Contains "Hel"
				.Concat(new byte[] { 0x80, 0x02, 0x6c, 0x6f }) // Contains "lo"
				.ToArray()
			);
			var Packet = await WebSocketPacket.ReadPacketFromStreamAsync(Version: 13, Stream: Stream);
			Assert.AreEqual("WebSocketPacket('TextFrame', 'Hello')", Packet.ToString());
		}

		[TestMethod]
		async public Task Test256BytesBinaryMessageInASingleUnmaskedFrame()
		{
			var Stream = GenerateBufferedStreamFromBytes(
				new byte[] { 0x82, 0x7E }
				.Concat(new byte[] { 0x01, 0x00 })
				.Concat(Enumerable.Range(0, 256).Select(Item => (byte)Item).ToArray()) // 0x00...0xFF
				.ToArray()
			);
			var Packet = await WebSocketPacket.ReadPacketFromStreamAsync(Version: 13, Stream: Stream);
			Assert.AreEqual("WebSocketPacket('TextFrame', 'Hello')", Packet.ToString());
		}
	}
}
