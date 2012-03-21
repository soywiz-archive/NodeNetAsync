using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Vfs;

namespace System
{
	static public class IVirtualFileSystemExtensions
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="VirtualFileSystem"></param>
		/// <param name="Path"></param>
		/// <returns></returns>
		async static public Task<byte[]> ReadAllBytesAsync(this IVirtualFileSystem VirtualFileSystem, VirtualFilePath Path)
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
		async static public Task<string> ReadAllContentAsStringAsync(this IVirtualFileSystem VirtualFileSystem, VirtualFilePath Path, Encoding Encoding)
		{
			var Bytes = await VirtualFileSystem.ReadAllBytesAsync(Path);
			if (Bytes.Length >= 3 && Bytes[0] == 0xEF && Bytes[1] == 0xBB && Bytes[2] == 0xBF)
			{
				return Encoding.GetString(Bytes, 3, Bytes.Length - 3);
			}
			else
			{
				return Encoding.GetString(Bytes);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="VirtualFileSystem"></param>
		/// <param name="Path"></param>
		/// <param name="Data"></param>
		/// <returns></returns>
		async static public Task WriteAllBytesAsync(this IVirtualFileSystem VirtualFileSystem, VirtualFilePath Path, byte[] Data)
		{
			using (var Stream = await VirtualFileSystem.OpenAsync(Path, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
			{
				await Stream.WriteAsync(Data, 0, Data.Length);
			}
		}

		async static public Task WriteAllContentAsync(this IVirtualFileSystem VirtualFileSystem, VirtualFilePath Path, String Data, Encoding Encoding)
		{
			await VirtualFileSystem.WriteAllBytesAsync(Path, Encoding.GetBytes(Data));
		}
	}
}
