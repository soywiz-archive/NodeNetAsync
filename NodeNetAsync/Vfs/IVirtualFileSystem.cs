using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Vfs
{
	public interface IVirtualFileSystem
	{
		Task<VirtualFileInfo> GetFileInfoAsync(VirtualFilePath Path);
		Task<IVirtualFileStream> OpenAsync(VirtualFilePath Path, FileMode FileMode, FileAccess FileAccess, FileShare FileShare);
		Task CreateDirectoryAsync(VirtualFilePath Path, DirectorySecurity DirectorySecurity);
		Task<IEnumerable<VirtualFileInfo>> EnumerateDirectoryAsync(VirtualFilePath Path);
		event Action<VirtualFileEvent> OnEvent;
	}
}
