using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NodeNetAsync.Streams
{
	public class ProxyStream : Stream
	{
		public Stream ParentStream;
		protected bool CloseParent;

		public ProxyStream(Stream ParentStream, bool CloseParent)
		{
			this.ParentStream = ParentStream;
			this.CloseParent = CloseParent;
		}

		public override bool CanRead
		{
			get { return ParentStream.CanRead; }
		}

		public override bool CanSeek
		{
			get { return ParentStream.CanSeek; }
		}

		public override bool CanWrite
		{
			get { return ParentStream.CanWrite; }
		}

		public override void Flush()
		{
			ParentStream.Flush();
		}

		public override long Length
		{
			get { return ParentStream.Length; }
		}

		public override long Position
		{
			get
			{
				return ParentStream.Position;
			}
			set
			{
				ParentStream.Position = value;
			}
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return ParentStream.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return ParentStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			ParentStream.SetLength(value);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			ParentStream.Write(buffer, offset, count);
		}

		public override Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
		{
			return base.WriteAsync(buffer, offset, count, cancellationToken);
		}

		public override Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
		{
			return base.ReadAsync(buffer, offset, count, cancellationToken);
		}

		protected override void Dispose(bool disposing)
		{
			if (CloseParent)
			{
				ParentStream.Dispose();
			}
		}

		public override void Close()
		{
			if (CloseParent)
			{
				ParentStream.Close();
			}
		}
	}
}
