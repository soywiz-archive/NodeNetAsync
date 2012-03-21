using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Vfs
{
	public struct VirtualFilePath
	{
		private string Value;

		/// <summary>
		/// /path/file.ext -> ext
		/// </summary>
		public string Extension
		{
			get
			{
				var Ext = Path.GetExtension(Value);
				if (Ext.Length > 0) return Ext.Substring(1);
				return Ext;
			}
		}

		/// <summary>
		/// /path/file.ext -> /path/file.ext
		/// </summary>
		public string FullPath
		{
			get
			{
				return Value;
			}
		}

		/// <summary>
		/// /path/file.ext -> /path/file
		/// </summary>
		public string FullPathWithoutExtension
		{
			get
			{
				var ExtensionIndex = Value.LastIndexOf(".");
				if (ExtensionIndex < 0) return Value;
				return Value.Substring(0, ExtensionIndex);
			}
		}

		static public implicit operator VirtualFilePath(string Value)
		{
			return new VirtualFilePath() { Value = Value };
		}

		static public implicit operator string(VirtualFilePath VirtualFilePath)
		{
			return VirtualFilePath.Value;
		}

		public override string ToString()
		{
			return FullPath;
		}

		static public string NormalizePath(string Path)
		{
			return NormalizePathInternal(Path.Replace('\\', '/'));
		}

		static private string NormalizePathInternal(string Path)
		{
			if (Path.Length == 0) return "";
			if (Path[0] == '/') return "/" + NormalizePathInternal(Path.TrimStart('/'));
			var NormalizedComponents = new Stack<string>();

			foreach (var Part in Path.Split('/'))
			{
				switch (Part)
				{
					case "":
					case ".":
						break;
					case "..":
						if (NormalizedComponents.Count > 0) NormalizedComponents.Pop();
						break;
					default:
						NormalizedComponents.Push(Part);
						break;
				}
			}

			return String.Join("/", NormalizedComponents);
		}

		public VirtualFilePath GetNormalized()
		{
			return NormalizePath(FullPath);
		}

		public string[] GetParts()
		{
			return FullPath.Split('/');
		}

		public bool IsAbsolute
		{
			get
			{
				return (FullPath.Length > 0) && (FullPath[0] == '/');
			}
		}
	}
}
