using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NodeNetAsync.OS
{
	public class FileSystem
	{
		/// <summary>
		/// 
		/// </summary>
		/// <param name="FileName"></param>
		/// <returns></returns>
		async static public Task<FileInfo> GetFileInfoAsync(string FileName)
		{
			return await Task.Run(() => new FileInfo(FileName));
		}
	}
}
