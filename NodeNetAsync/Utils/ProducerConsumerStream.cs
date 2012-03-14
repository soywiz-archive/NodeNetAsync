using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Utils
{
	public class ProducerConsumerStream : Stream
	{
		IProducerConsumer<byte> ProducerConsumer;

		public ProducerConsumerStream(IProducerConsumer<byte> ProducerConsumer)
		{
			this.ProducerConsumer = ProducerConsumer;
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return false; }
		}

		public override bool CanWrite
		{
			get { return true; }
		}

		public override void Flush()
		{
		}

		public override long Length
		{
			get
			{
				return ProducerConsumer.AvailableForRead;
			}
		}

		public override long Position
		{
			get
			{
				return 0;
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		async public override Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
		{
			//Console.WriteLine("a : {0} : {1}", ProducerConsumer.AvailableForRead, count);

			//var Task2 = new Task();
			//Console.WriteLine("[1]");
			await ProducerConsumer.OnData.Wait(() => (ProducerConsumer.AvailableForRead < 1));
			//Console.WriteLine("[2]");
			return ProducerConsumer.Read(buffer, offset, count);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return ProducerConsumer.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotImplementedException();
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			ProducerConsumer.Write(buffer, offset, count);
		}
	}
}
