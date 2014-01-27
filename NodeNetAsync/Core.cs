//#define RUN_ON_MONO

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace NodeNetAsync
{
	public class Core
	{
		public class CoreService : ServiceBase
		{
			Thread Thread;
			Func<Task> Action;

			public CoreService(Func<Task> Action)
			{
				this.Action = Action;
			}

			protected override void OnStart(string[] args)
			{
				Thread = new Thread(() =>
				{
					try
					{
						Action().Wait();
					}
					catch (Exception Exception)
					{
						Console.WriteLine(Exception);
					}

				});
				Thread.Start();
				base.OnStart(args);
			}

			protected override void OnStop()
			{
				Thread.Abort();
				base.OnStop();
			}
		}

		static public Version Version
		{
			get
			{
				return Assembly.GetAssembly(typeof(Core)).GetName().Version;
			}
		}

		static bool Debug
		{
			get
			{
#if DEBUG
				return true;
#else
				return false;
#endif
			}
		}

		static bool Standalone
		{
			get
			{
				return !Debugger.IsAttached;
			}
		}

		static public string GetExecutablePath()
		{
			return Assembly.GetEntryAssembly().Location;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Action"></param>
		static public void Loop(Func<Task> Action, bool ShowVersion = true)
		{
#if RUN_ON_MONO
			CheckAndRunOnMono();
#endif

#if false
			//if (Debugger.IsAttached)
			if (false)
			{
				Action().Wait();
			}
			else
#endif
			if (ShowVersion)
			{
				Console.Write("Node.NET {0}", Core.Version);
				if (Debug)
				{
					Console.Write(" - Debug Version");
				}

				if (Standalone)
				{
					Console.Write(" - Standalone");
				}
				else
				{
					Console.Write(" - Debugger Attached");
				}

				Console.Write(" - {0} Bits", IntPtr.Size * 8);

				Console.WriteLine();
				Console.WriteLine("Concurrency: {0}", TaskScheduler.Default.MaximumConcurrencyLevel);
			}

			//Console.WriteLine("c");

			//if (Environment.UserInteractive)
			if (true)
			{
				try
				{
					Action().Wait();
				}
				catch (Exception Exception)
				{
					Console.WriteLine(Exception);
					if (Debug)
					{
						Console.ReadKey();
					}
				}
			}
			else
			{
				ServiceBase.Run(new CoreService(Action));
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Action"></param>
		/// <param name="TimeSpan"></param>
		/// <returns></returns>
		static public TimerAsync SetTimeout(Func<Task> Action, TimeSpan TimeSpan)
		{
			var TimerAsync = new TimerAsync();
			Task.Run(async () =>
			{
				await Task.Delay(TimeSpan);
				if (TimerAsync.Running) await Action();
			});
			return TimerAsync;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Action"></param>
		/// <param name="TimeSpan"></param>
		/// <returns></returns>
		static public TimerAsync SetInterval(Func<Task> Action, TimeSpan TimeSpan)
		{
			var TimerAsync = new TimerAsync();
			Task.Run(async () =>
			{
				while (TimerAsync.Running)
				{
					await Task.Delay(TimeSpan);
					if (TimerAsync.Running) await Action();
				}
			});
			return TimerAsync;
		}

		/*
		static public TimerAsync SetTimeout(Action Action, TimeSpan TimeSpan)
		{
			return SetTimeout(() => new Task(Action), TimeSpan);
		}

		static public TimerAsync SetInterval(Action Action, TimeSpan TimeSpan)
		{
			return SetInterval(() => new Task(Action), TimeSpan);
		}
		*/

		/// <summary>
		/// 
		/// </summary>
		/// <param name="TimerAsync"></param>
		static public void ClearTimeout(TimerAsync TimerAsync)
		{
			TimerAsync.Running = false;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="TimerAsync"></param>
		static public void ClearInterval(TimerAsync TimerAsync)
		{
			TimerAsync.Running = false;
		}

		/// <summary>
		/// 
		/// </summary>
		public class TimerAsync
		{
			internal bool Running = true;
		}
	}
}
