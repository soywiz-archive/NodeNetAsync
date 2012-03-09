using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Streams
{
	public interface IAsyncWriter
	{
		Task WriteAsync(byte[] Data, int Offset = 0, int Count = -1);
	}
}
