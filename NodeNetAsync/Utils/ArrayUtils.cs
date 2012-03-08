using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Utils
{
	public class ArrayUtils
	{
		static public byte[] GetSlice(byte[] Data, int Offset, int Count)
		{
			var Return = new byte[Count];
			Array.Copy(Data, Offset, Return, 0, Count);
			return Return;
		}
	}
}
