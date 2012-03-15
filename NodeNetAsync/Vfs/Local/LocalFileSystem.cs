using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
			return Url.GetInnerFileRelativeToPath(RootPath, RelativePath);
		}

		protected string GetRelativePathFromAbsolute(string AbsolutePath)
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

		async public Task<VirtualFileInfo> GetFileInfoAsync(VirtualFilePath Path)
		{
			return await Task.Run(() =>
			{
				try
				{
					var FileInfo = new FileInfo(GetAbsolutePathFromRelative(Path));
					return new VirtualFileInfo()
					{
						Length = FileInfo.Length,
						Exists = FileInfo.Exists,
						LastWriteTimeUtc = FileInfo.LastWriteTimeUtc,
					};
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

		async public Task<VirtualFileStream> OpenAsync(VirtualFilePath Path, FileMode FileMode, FileAccess FileAccess, FileShare FileShare)
		{
			Exception YieldedException = null;

			var Result = await Task.Run(() =>
			{
				try
				{
					return File.Open(GetAbsolutePathFromRelative(Path), FileMode, FileAccess, FileShare);
				}
				catch (Exception Exception)
				{
					YieldedException = Exception;
					return null;
				}
			});

			if (YieldedException != null) throw YieldedException;

			return Result;
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
	}
}
