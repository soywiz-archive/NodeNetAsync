using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NodeNetAsync.Utils
{
	/// <summary>
	/// 
	/// </summary>
	public class AsyncTaskEventWaiter
	{
		protected Queue<CancellationTokenSource> Waiting = new Queue<CancellationTokenSource>();

		/// <summary>
		/// Wait for a signal
		/// </summary>
		/// <returns></returns>
		async public Task Wait(Func<bool> WaitWhileCondition)
		{
			CancellationTokenSource TokenSource = null;

			//Console.WriteLine("Thread.Wait: {0}", Thread.CurrentThread.ManagedThreadId);
			try
			{
				while (WaitWhileCondition())
				{
					if (TokenSource == null)
					{
						TokenSource = new CancellationTokenSource();
						Waiting.Enqueue(TokenSource);
					}

					await Task.Delay(10, TokenSource.Token);
				}
			}
			catch (TaskCanceledException)
			{
			}
		}

		/// <summary>
		/// Awake all tasks waiting for this event
		/// </summary>
		public void Signal()
		{
			//lock (this)
			{
				//Console.WriteLine("Thread.Signal: {0}", Thread.CurrentThread.ManagedThreadId);
				while (Waiting.Count > 0) Waiting.Dequeue().Cancel();
			}
		}
	}
}
