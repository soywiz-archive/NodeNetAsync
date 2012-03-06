using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NodeNetAsync
{
	public class Core
	{
		static public void Loop(Func<Task> Action)
		{
			try
			{
				Action().Wait();
			}
			catch (Exception Exception)
			{
				Console.WriteLine(Exception);
			}
			while (true) Thread.Sleep(int.MaxValue);
		}
	}
}
