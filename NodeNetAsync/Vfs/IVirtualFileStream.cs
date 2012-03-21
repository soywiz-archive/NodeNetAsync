using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Vfs
{
	public interface IVirtualFileStream : IDisposable
	{
		long Length { get; }
		Task<int> ReadAsync(byte[] Buffer, int Offset, int Count);
		Task WriteAsync(byte[] Buffer, int Offset, int Count);
		Stream SystemStream { get; }
	}
}
