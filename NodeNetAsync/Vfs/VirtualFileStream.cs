using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Vfs
{
	public class VirtualFileStream : IVirtualFileStream
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

		async public Task WriteAsync(byte[] Buffer, int Offset, int Count)
		{
			await Stream.WriteAsync(Buffer, Offset, Count);
		}

		/*
		static public implicit operator IVirtualFileStream(Stream Stream)
		{
			return new VirtualFileStream(Stream);
		}

		static public implicit operator Stream(IVirtualFileStream VirtualFileStream)
		{
			return VirtualFileStream.Stream;
		}
		*/

		public void Dispose()
		{
			this.Stream.Dispose();
		}

		public Stream SystemStream
		{
			get
			{
				return this.Stream;
			}
		}
	}
}
