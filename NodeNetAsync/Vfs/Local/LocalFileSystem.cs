using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Utils;

namespace NodeNetAsync.Vfs.Local
{
	public class LocalFileSystem : IVirtualFileSystem
	{
		string RootPath;
		FileSystemWatcher FileSystemWatcher;

		public LocalFileSystem(string RootPath)
		{
			this.RootPath = RootPath = Url.Normalize(RootPath);
			this.FileSystemWatcher = new FileSystemWatcher(RootPath);
			this.FileSystemWatcher.IncludeSubdirectories = true;
			this.FileSystemWatcher.Created += (sender, e) => { _OnEvent(new VirtualFileEvent() { Type = VirtualFileEvent.TypeEnum.Created, File1 = GetRelativePathFromAbsolute(e.FullPath) }); };
			this.FileSystemWatcher.Changed += (sender, e) => { _OnEvent(new VirtualFileEvent() { Type = VirtualFileEvent.TypeEnum.Updated, File1 = GetRelativePathFromAbsolute(e.FullPath) }); };
			this.FileSystemWatcher.Deleted += (sender, e) => { _OnEvent(new VirtualFileEvent() { Type = VirtualFileEvent.TypeEnum.Deleted, File1 = GetRelativePathFromAbsolute(e.FullPath) }); };
			this.FileSystemWatcher.Renamed += (sender, e) => { _OnEvent(new VirtualFileEvent() { Type = VirtualFileEvent.TypeEnum.Renamed, File1 = GetRelativePathFromAbsolute(e.OldFullPath), File2 = GetRelativePathFromAbsolute(e.FullPath) }); };
		}

		protected string GetAbsolutePathFromRelative(string RelativePath)
		{
			return Url.Merge(RootPath, RelativePath);
		}

		protected VirtualFilePath GetRelativePathFromAbsolute(string AbsolutePath)
		{
			AbsolutePath = Url.Normalize(AbsolutePath);
			if (AbsolutePath.StartsWith(RootPath))
			{
				return AbsolutePath.Substring(RootPath.Length);
			}
			else
			{
				throw(new InvalidOperationException("Absolute path outside this LocalFileSystem"));
			}
		}

		private VirtualFileInfo ConvertFileSystemInfoToVirtualFileInfo(FileSystemInfo FileSystemInfo)
		{
			var VirtualFileInfo = new VirtualFileInfo()
			{
				Path = GetRelativePathFromAbsolute(FileSystemInfo.FullName),
				Exists = FileSystemInfo.Exists,
				LastWriteTimeUtc = FileSystemInfo.LastWriteTimeUtc,
			};

			var FileInfo = FileSystemInfo as FileInfo;
			var DirectoryInfo = FileSystemInfo as DirectoryInfo;

			if (FileInfo != null)
			{
				VirtualFileInfo.Length = FileInfo.Length;
				VirtualFileInfo.Type = VirtualFileType.File;
			}
			else if (DirectoryInfo != null)
			{
				VirtualFileInfo.Length = 0;
				VirtualFileInfo.Type = VirtualFileType.Directory;
			}

			return VirtualFileInfo;
		}

		async public Task<VirtualFileInfo> GetFileInfoAsync(VirtualFilePath Path)
		{
			return await Task.Run(() =>
			{
				try
				{
					return ConvertFileSystemInfoToVirtualFileInfo(new FileInfo(GetAbsolutePathFromRelative(Path)));
				}
				catch
				{
					return new VirtualFileInfo()
					{
						Exists = false,
					};
				}
			});
		}

		async public Task<IVirtualFileStream> OpenAsync(VirtualFilePath Path, FileMode FileMode, FileAccess FileAccess, FileShare FileShare)
		{
			return await TaskEx.RunPropagatingExceptionsAsync(() =>
			{
				var Stream = File.Open(GetAbsolutePathFromRelative(Path), FileMode, FileAccess, FileShare);
				return new VirtualFileStream(Stream);
			});
		}

		async public Task<IEnumerable<VirtualFileInfo>> EnumerateDirectoryAsync(VirtualFilePath Path)
		{
			return await TaskEx.RunPropagatingExceptionsAsync(() =>
			{
				var List = new List<VirtualFileInfo>();
				foreach (var FileSystemInfo in new DirectoryInfo(GetAbsolutePathFromRelative(Path)).EnumerateFileSystemInfos())
				{
					List.Add(ConvertFileSystemInfoToVirtualFileInfo(FileSystemInfo));
				}
				return List;
			});
		}

		private event Action<VirtualFileEvent> _OnEvent;

		private void UpdatedOnEvent()
		{
			this.FileSystemWatcher.EnableRaisingEvents = (_OnEvent != null);
		}

		public event Action<VirtualFileEvent> OnEvent
		{
			add
			{
				_OnEvent += value;
				UpdatedOnEvent();
			}
			remove
			{
				_OnEvent -= value;
				UpdatedOnEvent();
			}
		}

		async public Task CreateDirectoryAsync(VirtualFilePath Path, DirectorySecurity DirectorySecurity)
		{
			await Task.Yield();
			Directory.CreateDirectory(GetAbsolutePathFromRelative(Path), DirectorySecurity);
		}
	}
}
