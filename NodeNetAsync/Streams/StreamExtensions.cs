using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
	static public class StreamExtensions
	{
		static public void SaveRestorePositionAndLock(this Stream Stream, Action Action)
		{
			lock (Stream)
			{
				var OldPosition = Stream.Position;
				try
				{
					Action();
				}
				finally
				{
					Stream.Position = OldPosition;
				}
			}
		}

		async static public Task SaveRestorePositionAsync(this Stream Stream, Func<Task> Action)
		{
			var OldPosition = Stream.Position;
			try
			{
				await Action();
			}
			finally
			{
				Stream.Position = OldPosition;
			}
		}
	}
}
