using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NodeNetAsync.Net.Http;
using NodeNetAsync.OS;

namespace NodeNetAsync.Examples
{
	public class SetIntervalTestProgram
	{
		static void Main(string[] args)
		{
			Core.Loop(async () =>
			{
				int SecondsElapsed = 0;

				Core.SetInterval(async () =>
				{
					SecondsElapsed++;
				}, TimeSpan.FromMilliseconds(1000));

				/*
				var DirProcess = await Process.SpawnAsync(@"c:\dev\php\php.exe", "-v");
				await Console.Out.WriteLineAsync(await DirProcess.StandardOutput.ReadLineAsync(Encoding.UTF8));
				await Console.Out.WriteLineAsync(await DirProcess.StandardOutput.ReadLineAsync(Encoding.UTF8));
				await DirProcess.WaitForExitAsync();
				*/

				await HttpServer.Create(async (Request, Response) =>
				{
					await Response.WriteAsync("Seconds Elapsed: " + SecondsElapsed);
					//GC.Collect();
				}).ListenAsync(80);
			});
		}
	}
}
