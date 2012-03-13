using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Utils
{
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
