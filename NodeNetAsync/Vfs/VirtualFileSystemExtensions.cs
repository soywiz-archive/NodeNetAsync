using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Vfs
{
	static public class VirtualFileSystemExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="VirtualFileSystem"></param>
		/// <param name="Path"></param>
		/// <returns></returns>
		async static public Task<byte[]> ReadAllBytesAsync(this IVirtualFileSystem VirtualFileSystem, string Path)
		{
			using (var Stream = await VirtualFileSystem.OpenAsync(Path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
			{
				var Data = new byte[Stream.Length];
				await Stream.ReadAsync(Data, 0, Data.Length);
				return Data;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="VirtualFileSystem"></param>
		/// <param name="Path"></param>
		/// <param name="Encoding"></param>
		/// <returns></returns>
		async static public Task<string> ReadAllContentAsStringAsync(this IVirtualFileSystem VirtualFileSystem, string Path, Encoding Encoding)
		{
			return Encoding.GetString(await VirtualFileSystem.ReadAllBytesAsync(Path));
		}
	}
}
