using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Utils
{
	public class AsyncCache<TKey, TValue>
	{
		public class Entry
		{
			public Task<TValue> Task;
			public TValue Value;
		}

		protected ConcurrentDictionary<TKey, Entry> Items = new ConcurrentDictionary<TKey, Entry>();

		private bool _Enabled;

		public bool Enabled {
			get
			{
				return _Enabled;
			}
			set
			{
				_Enabled = value;
				if (_Enabled == false) Items.Clear();
			}
		}

		public AsyncCache(bool Enabled = true)
		{
			this.Enabled = Enabled;
		}

		async public Task<TValue> GetAsync(TKey Key, Func<Task<TValue>> Generator)
		{
			Entry CurrentEntry;

			// Already in dictionary. Generating or generated already.
			if (Items.TryGetValue(Key, out CurrentEntry))
			{
				if (CurrentEntry.Task != null) await CurrentEntry.Task;
			}
			// Not in dictionary yet.
			else
			{
				CurrentEntry = new Entry();
				if (Enabled) Items[Key] = CurrentEntry;
				
				CurrentEntry.Task = Generator();
				CurrentEntry.Value = await CurrentEntry.Task;
				CurrentEntry.Task = null;
			}

			return CurrentEntry.Value;
		}

		public void Remove(TKey Key)
		{
			Entry Value;
			Items.TryRemove(Key, out Value);
		}
	}
}
