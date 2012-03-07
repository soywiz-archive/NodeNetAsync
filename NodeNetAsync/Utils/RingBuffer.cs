using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Utils
{
	public class RingBuffer<TType>
	{
		private int PositionWrite;
		private int PositionRead;
		public int AvailableForRead { get; private set; }
		public int AvailableForWrite { get; private set; }
		private TType[] Data;
		public readonly int Size;

		public RingBuffer(int Size)
		{
			this.Size = Size;
			this.Data = new TType[Size];
			this.AvailableForWrite = Size;
			this.AvailableForRead = 0;
		}

		public TType[] PeekGet(int Count)
		{
			var Temp = new TType[Count];
			int Readed = Peek(Temp, 0, Count);
			var Return = new TType[Readed];
			Array.Copy(Temp, 0, Return, 0, Readed);
			return Return;
		}

		public int Peek(TType[] Buffer, int Offset = 0, int Count = -1)
		{
			if (Count < 0) Count = Buffer.Length;
			int TempPositionRead = PositionRead;
			Count = Math.Min(Count, AvailableForRead);
			if (Buffer != null)
			{
				for (int n = 0; n < Count; n++)
				{
					Buffer[Offset + n] = Data[TempPositionRead++];
					if (TempPositionRead >= Size) TempPositionRead = 0;
				}
			}
			return Count;
		}

		public int Skip(int Count)
		{
			Count = Math.Min(Count, AvailableForRead);
			PositionRead = (PositionRead + Count) % Size;
			AvailableForRead -= Count;
			AvailableForWrite += Count;
			return Count;
		}

		public int Read(TType[] Buffer, int Offset = 0, int Count = -1)
		{
			return Skip(Peek(Buffer, Offset, Count));
		}

		public void Write(TType[] Buffer, int Offset = 0, int Count = -1)
		{
			if (Count < 0) Count = Buffer.Length;
			Count = Math.Min(Count, AvailableForWrite);

			for (int n = 0; n < Count; n++)
			{
				Data[PositionWrite++] = Buffer[Offset + n];
				if (PositionWrite >= Size) PositionWrite = 0;
			}

			AvailableForWrite -= Count;
			AvailableForRead += Count;
		}
	}
}
