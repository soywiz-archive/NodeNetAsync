using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Utils
{
	public interface IProducerConsumer<TType>
	{
		int AvailableForRead { get; }
		int AvailableForWrite { get; }

		AsyncTaskEventWaiter OnData { get; }

		int Skip(int Count);
		int Peek(TType[] Buffer, int Offset = 0, int Count = -1);
		void Write(TType[] Buffer, int Offset = 0, int Count = -1);
	}

	static public class IProducerConsumerExtensions
	{
		static public TType[] PeekGet<TType>(this IProducerConsumer<TType> ProduceConsumer, int Count)
		{
			var Temp = new TType[Count];
			int Readed = ProduceConsumer.Peek(Temp, 0, Count);
			var Return = new TType[Readed];
			Array.Copy(Temp, 0, Return, 0, Readed);
			return Return;
		}

		static public int Read<TType>(this IProducerConsumer<TType> ProduceConsumer, TType[] Buffer, int Offset = 0, int Count = -1)
		{
			return ProduceConsumer.Skip(ProduceConsumer.Peek(Buffer, Offset, Count));
		}
	}
}
