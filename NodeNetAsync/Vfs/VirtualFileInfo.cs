using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Vfs
{
	public enum VirtualFileType
	{
		File = 0,
		Directory = 1,
		Device = 2,
	}

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

		/// <summary>
		/// 
		/// </summary>
		public VirtualFileType Type;

		/// <summary>
		/// 
		/// </summary>
		public bool IsDirectory { get { return Type == VirtualFileType.Directory; } }

		/// <summary>
		/// 
		/// </summary>
		public bool IsFile { get { return Type == VirtualFileType.File; } }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return String.Format("VirtualFileInfo('{0}')", Path);
		}
	}
}
