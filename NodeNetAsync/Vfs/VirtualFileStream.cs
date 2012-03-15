using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Vfs
{
	public class VirtualFileStream : IDisposable
	{
		Stream Stream;

		public VirtualFileStream(Stream Stream)
		{
			this.Stream = Stream;
		}

		public long Length
		{
			get { return Stream.Length; }
		}

		async public Task<int> ReadAsync(byte[] Buffer, int Offset, int Count)
		{
			return await Stream.ReadAsync(Buffer, Offset, Count);
		}

		static public implicit operator VirtualFileStream(Stream Stream)
		{
			return new VirtualFileStream(Stream);
		}

		static public implicit operator Stream(VirtualFileStream VirtualFileStream)
		{
			return VirtualFileStream.Stream;
		}

		public void Dispose()
		{
			this.Stream.Dispose();
		}
	}
}
