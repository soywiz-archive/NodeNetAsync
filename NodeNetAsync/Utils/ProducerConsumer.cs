using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Utils
{
	public class ProducerConsumer<TType> : IProducerConsumer<TType>
	{
		TType[] List = new TType[0];

		public ProducerConsumer()
		{
			OnData = new AsyncTaskEventWaiter();
		}

		public int Peek(TType[] Buffer, int Offset = 0, int Count = -1)
		{
			if (Count < 0) Count = Buffer.Length - Offset;
			Count = Math.Min(Count, List.Length);
			if (Count > 0)
			{
				Array.Copy(this.List, 0, Buffer, Offset, Count);
			}
			return Count;
		}

		public int Skip(int Count)
		{
			Count = Math.Min(Count, List.Length);
			var NewList = new TType[List.Length - Count];
			if (NewList.Length > 0)
			{
				Array.Copy(this.List, Count, NewList, 0, NewList.Length);
			}
			this.List = NewList;
			return Count;
		}

		public void Write(TType[] Buffer, int Offset = 0, int Count = -1)
		{
			if (Count < 0) Count = Buffer.Length - Offset;
			var NewList = new TType[this.List.Length + Count];
			if (List.Length > 0)
			{
				Array.Copy(List, 0, NewList, 0, List.Length);
			}
			if (Count > 0)
			{
				Array.Copy(Buffer, Offset, NewList, List.Length, Count);
			}
			this.List = NewList;

			if (Count > 0)
			{
				OnData.Signal();
			}
		}

		public int AvailableForRead
		{
			get { return List.Length; }
		}

		public int AvailableForWrite
		{
			get { return 8 * 1024 * 1024; }
		}


		public AsyncTaskEventWaiter OnData
		{
			get;
			protected set;
		}
	}
}
