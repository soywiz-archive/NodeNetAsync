using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Utils;

namespace System.Linq
{
	static public class Extensions
	{
		static public byte[] GetContentBytes(this MemoryStream MemoryStream)
		{
			return ArrayUtils.GetSlice(MemoryStream.GetBuffer(), 0, (int)MemoryStream.Length);
		}
	}
}
