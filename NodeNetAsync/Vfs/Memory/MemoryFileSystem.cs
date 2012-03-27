using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Streams;

namespace NodeNetAsync.Vfs.Memory
{
	public class MemoryFileSystem : IVirtualFileSystem
	{
		public class NodeVirtualFileInfo : VirtualFileInfo
		{
			protected string Name;

			public NodeVirtualFileInfo(string Name, VirtualFileType Type, NodeVirtualFileInfo Parent = null)
			{
				this.Name = Name;
				this.Type = Type;
				this.Parent = Parent;
			}

			public Stream Stream = new MemoryStream();
			public NodeVirtualFileInfo Parent;
			public List<NodeVirtualFileInfo> Childs = new List<NodeVirtualFileInfo>();
			private Dictionary<string, NodeVirtualFileInfo> ChildsByName = new Dictionary<string, NodeVirtualFileInfo>();

			public NodeVirtualFileInfo ParentOrThis { get { return (Parent != null) ? Parent : this; } }
			public NodeVirtualFileInfo Root { get { return (Parent != null) ? Parent.Root : this; } }

			private NodeVirtualFileInfo LocateParentNode(VirtualFilePath Path, out string LastPartName)
			{
				Path = Path.GetNormalized();

				if (Path.IsAbsolute)
				{
					return Root.LocateParentNode(Path.FullPath.Substring(1), out LastPartName);
				}

				NodeVirtualFileInfo CurrentDirectory = this;
				var Parts = Path.GetParts();
				if (Parts.Length < 1)
				{
					LastPartName = "";
					return this;
				}
				for (int n = 0; n < Parts.Length - 1; n++)
				{
					var Part = Parts[n];
					CurrentDirectory = CurrentDirectory.GetChild(Part);
					if (CurrentDirectory == null || !CurrentDirectory.IsDirectory) throw (new FileNotFoundException("Component '" + Part + "' is not a directory in '" + Path + "'", Path));
				}

				LastPartName = Parts[Parts.Length - 1];

				return CurrentDirectory;
			}

			internal NodeVirtualFileInfo Open(VirtualFilePath Path)
			{
				string FileName;
				var ParentDirectory = LocateParentNode(Path, out FileName);
				if (!ParentDirectory.ContainsChild(FileName))
				{
					throw (new IOException("File '" + Path + "' : '" + FileName + "' doesn't Exist"));
				}
				return ParentDirectory.GetChild(FileName);
			}

			internal NodeVirtualFileInfo Create(VirtualFilePath Path, VirtualFileType CreateType, bool ErrorIfExists)
			{
				string FileName;
				var ParentDirectory = LocateParentNode(Path, out FileName);
				if (ParentDirectory.ContainsChild(FileName))
				{
					if (ErrorIfExists)
					{
						throw (new IOException("File Already Exists"));
					}
					else
					{
						return ParentDirectory.GetChild(FileName);
					}
				}
				else
				{
					return ParentDirectory.AddChild(FileName, CreateType);
				}
			}

			private bool ContainsChild(string FileName)
			{
				return this.ChildsByName.ContainsKey(FileName);
			}

			private NodeVirtualFileInfo AddChild(string Part, VirtualFileType Type)
			{
				var That = new NodeVirtualFileInfo(Part, Type, this);
				{
					this.Childs.Add(That);
					this.ChildsByName.Add(Part, That);
				}
				return That;
			}

			private NodeVirtualFileInfo GetChild(string Part)
			{
				NodeVirtualFileInfo Out;
				return ChildsByName.TryGetValue(Part, out Out) ? Out : null;
			}

			async public Task<IVirtualFileStream> OpenAsync(FileMode FileMode, FileAccess FileAccess, FileShare FileShare)
			{
				await Task.Yield();
				return new VirtualFileStream(new ProxyStream(this.Stream, CloseParent: false));
			}
		}

		NodeVirtualFileInfo Root = new NodeVirtualFileInfo(null, VirtualFileType.Directory);

		async public Task<VirtualFileInfo> GetFileInfoAsync(VirtualFilePath Path)
		{
			await Task.Yield();
			return Root.Open(Path);
		}

		async public Task<IVirtualFileStream> OpenAsync(VirtualFilePath Path, FileMode OpenFileMode, FileAccess FileAccess, FileShare FileShare)
		{
			NodeVirtualFileInfo Node;

			switch (OpenFileMode)
			{
				case FileMode.Create:
				case FileMode.CreateNew:
				case FileMode.OpenOrCreate:
				case FileMode.Append:
					Node = Root.Create(Path, CreateType: VirtualFileType.File, ErrorIfExists: OpenFileMode == FileMode.CreateNew);
					break;
				default:
				case FileMode.Open:
				case FileMode.Truncate:
					Node = Root.Open(Path);
					break;
			}
			return await Node.OpenAsync(OpenFileMode, FileAccess, FileShare);
		}

		async public Task<IEnumerable<VirtualFileInfo>> EnumerateDirectoryAsync(VirtualFilePath Path)
		{
			await Task.Yield();
			return Root.Open(Path).Childs;
		}

		public event Action<VirtualFileEvent> OnEvent
		{
			add { }
			remove { }
		}

		async public Task CreateDirectoryAsync(VirtualFilePath Path, System.Security.AccessControl.DirectorySecurity DirectorySecurity)
		{
			await Task.Yield();
			Root.Create(Path, CreateType: VirtualFileType.Directory, ErrorIfExists: true);
		}
	}
}
