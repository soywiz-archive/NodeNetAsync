using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Utils
{
	public class Cache<TKey, TValue>
	{
		public class Entry
		{
			public Task<TValue> Task;
			public TValue Value;
		}

		protected Dictionary<TKey, Entry> Items = new Dictionary<TKey, Entry>();

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
				CurrentEntry.Task = Generator();
				CurrentEntry.Value = await CurrentEntry.Task;
				CurrentEntry.Task = null;
				if (Enabled) Items[Key] = CurrentEntry;
			}

			return CurrentEntry.Value;
		}
	}
}
