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
		static public TType[] GetSlice<TType>(this TType[] Data, int Offset, int Count)
		{
			var Return = new TType[Count];
			Array.Copy(Data, Offset, Return, 0, Count);
			return Return;
		}

		static public byte[] GetContentBytes(this MemoryStream MemoryStream)
		{
			return MemoryStream.ToArray();
			//return MemoryStream.GetBuffer().GetSlice(0, (int)MemoryStream.Length);
		}
	}
}
