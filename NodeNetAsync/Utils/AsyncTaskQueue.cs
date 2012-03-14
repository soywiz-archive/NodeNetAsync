using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Utils
{
	/// <summary>
	/// 
	/// </summary>
	/// <see cref="http://msdn.microsoft.com/en-us/library/system.threading.tasks.taskscheduler(v=vs.110).aspx"/>
	public class AsyncTaskQueue
	{
		protected Task LastEnqueuedTask;

		async public Task EnqueueAsync(Func<Task> TaskToEnqueue)
		{
			var PreviousEnqueuedTask = LastEnqueuedTask;
			LastEnqueuedTask = ((Func<Task>)(async () =>
			{
				if (PreviousEnqueuedTask != null) await PreviousEnqueuedTask;
				await TaskToEnqueue();
			}))();

			await LastEnqueuedTask;
			LastEnqueuedTask = null;
		}

		async public Task WaitAllAsync()
		{
			if (LastEnqueuedTask != null)
			{
				await LastEnqueuedTask;
			}
		}
	}
}
