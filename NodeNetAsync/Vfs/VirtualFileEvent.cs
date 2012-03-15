using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Vfs
{
	public class VirtualFileEvent
	{
		public enum TypeEnum
		{
			Created = 0,
			Updated = 1,
 			Renamed = 2,
			Deleted = 3,
		}

		public TypeEnum Type;
		public string File1;
		public string File2;
	}
}
