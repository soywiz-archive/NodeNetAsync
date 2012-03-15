using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Vfs
{
	public interface IVirtualFileSystem
	{
		Task<VirtualFileInfo> GetFileInfoAsync(VirtualFilePath Path);
		Task<VirtualFileStream> OpenAsync(VirtualFilePath Path, FileMode FileMode, FileAccess FileAccess, FileShare FileShare);
		event Action<VirtualFileEvent> OnEvent;
	}
}
