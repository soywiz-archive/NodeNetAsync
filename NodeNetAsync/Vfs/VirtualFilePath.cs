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

		static public implicit operator VirtualFilePath(string Value)
		{
			return new VirtualFilePath() { Value = Value };
		}

		static public implicit operator string(VirtualFilePath VirtualFilePath)
		{
			return VirtualFilePath.Value;
		}
	}
}
