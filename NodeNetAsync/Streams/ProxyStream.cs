using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.IO
{
	public class ProxyStream : Stream
	{
		public long InternalPosition;
		public Stream ParentStream;
		protected bool CloseParent;

		public ProxyStream(Stream ParentStream, bool CloseParent)
		{
			if (ParentStream is ProxyStream) ParentStream = ((ProxyStream)ParentStream).ParentStream;
			this.ParentStream = ParentStream;
			this.CloseParent = CloseParent;
			this.InternalPosition = 0;
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
				return InternalPosition;
			}
			set
			{
				InternalPosition = value;
			}
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			switch (origin)
			{
				case SeekOrigin.Begin: Position = offset; break;
				case SeekOrigin.Current: Position += offset; break;
				case SeekOrigin.End: Position = Length + offset; break;
			}
			return Position;
		}

		public override void SetLength(long value)
		{
			ParentStream.SetLength(value);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int Result = 0;
			ParentStream.SaveRestorePositionAndLock(() =>
			{
				ParentStream.Position = Position;
				Result = ParentStream.Read(buffer, offset, count);
			});
			return Result;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			ParentStream.SaveRestorePositionAndLock(() =>
			{
				ParentStream.Position = Position;
				ParentStream.Write(buffer, offset, count);
			});
		}

		async public override Task WriteAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
		{
			await ParentStream.SaveRestorePositionAsync(async () =>
			{
				ParentStream.Position = Position;
				await ParentStream.WriteAsync(buffer, offset, count, cancellationToken);
			});
		}

		async public override Task<int> ReadAsync(byte[] buffer, int offset, int count, System.Threading.CancellationToken cancellationToken)
		{
			int Result = 0;
			await ParentStream.SaveRestorePositionAsync(async () =>
			{
				ParentStream.Position = Position;
				Result = await ParentStream.ReadAsync(buffer, offset, count, cancellationToken);
			});
			return Result;
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
