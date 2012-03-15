using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Vfs
{
	public class VirtualFileInfo
	{
		/// <summary>
		/// 
		/// </summary>
		public VirtualFilePath Path;

		/// <summary>
		/// 
		/// </summary>
		public long Length;

		/// <summary>
		/// 
		/// </summary>
		public bool Exists;

		/// <summary>
		/// 
		/// </summary>
		public DateTime LastWriteTimeUtc;
	}
}
