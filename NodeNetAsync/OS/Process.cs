using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NodeNetAsync.Net;
using NodeNetAsync.Streams;
using SystemProcess = System.Diagnostics.Process;

namespace NodeNetAsync.OS
{
	public class Process
	{
		SystemProcess SystemProcess;

		public NodeBufferedStream Out { get; protected set; }
		public NodeBufferedStream Error { get; protected set; }

		async static public Task<Process> SpawnAsync(string Command, params string[] Arguments)
		{
			var Process = new Process();
			await Process.StartAsync(Command, Arguments);
			return Process;
		}

		protected Process()
		{
		}

		async protected Task StartAsync(string Command, params string[] Arguments)
		{
			await Task.Run(() =>
			{
				var StartInfo = new ProcessStartInfo(Command, String.Join(" ", Arguments)); ;
				StartInfo.CreateNoWindow = true;
				StartInfo.UseShellExecute = false;
				StartInfo.RedirectStandardError = true;
				StartInfo.RedirectStandardInput = true;
				StartInfo.RedirectStandardOutput = true;
				this.SystemProcess = SystemProcess.Start(StartInfo);
				this.SystemProcess.EnableRaisingEvents = true;
				this.Out = new NodeBufferedStream(this.SystemProcess.StandardOutput.BaseStream);
				this.Error = new NodeBufferedStream(this.SystemProcess.StandardError.BaseStream);
				this.SystemProcess.Exited += SystemProcess_Exited;
			});
		}

		void SystemProcess_Exited(object sender, EventArgs e)
		{
			EndedCancellationToken.Cancel();
		}

		CancellationTokenSource EndedCancellationToken = new CancellationTokenSource();

		async public Task WaitForExitAsync()
		{
			try
			{
				await Task.Delay(int.MaxValue, EndedCancellationToken.Token);
			}
			catch (TaskCanceledException)
			{
			}
		}
	}
}
