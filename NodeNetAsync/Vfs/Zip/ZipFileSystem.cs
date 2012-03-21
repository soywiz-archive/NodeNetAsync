using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Vfs.Zip
{
	internal class ZipVirtualFileInfo : VirtualFileInfo
	{
	}

	public class ZipFileSystem : IVirtualFileSystem
	{
		bool Parsed;
		IVirtualFileStream ZipStream;

		public ZipFileSystem(IVirtualFileStream ZipStream)
		{
			this.ZipStream = ZipStream;
		}

		async public Task ParseZipOnceAsync()
		{
			await Task.Yield();
			if (!Parsed)
			{
				Parsed = true;
				throw new NotImplementedException();
			}
		}

		async public Task<VirtualFileInfo> GetFileInfoAsync(VirtualFilePath Path)
		{
			await ParseZipOnceAsync();

			return await TaskEx.RunPropagatingExceptionsAsync<VirtualFileInfo>(() =>
			{
				throw new NotImplementedException();
				//return default(VirtualFileInfo);
			});
		}

		async public Task<IVirtualFileStream> OpenAsync(VirtualFilePath Path, System.IO.FileMode FileMode, System.IO.FileAccess FileAccess, System.IO.FileShare FileShare)
		{
			await ParseZipOnceAsync();
			throw new NotImplementedException();
		}

		async public Task<IEnumerable<VirtualFileInfo>> EnumerateDirectoryAsync(VirtualFilePath Path)
		{
			await ParseZipOnceAsync();
			throw new NotImplementedException();
		}

		public event Action<VirtualFileEvent> OnEvent
		{
			add { }
			remove { }
		}

		public Task CreateDirectoryAsync(VirtualFilePath Path, System.Security.AccessControl.DirectorySecurity DirectorySecurity)
		{
			throw new NotImplementedException();
		}
	}
}
