using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System
{
	public class IKVMReader : java.io.Reader
	{
		TextReader TextReader;

		public IKVMReader(TextReader TextReader)
		{
			this.TextReader = TextReader;
		}

		public override void close()
		{
			//throw new NotImplementedException();
		}

		public override int read(char[] charr, int index, int count)
		{
			int Return = this.TextReader.Read(charr, index, count);
			//Console.WriteLine(Return);
			if (Return == 0) return -1;
			return Return;
		}
	}

	public class IKVMWriter : java.io.Writer
	{
		TextWriter TextWriter;

		public IKVMWriter(TextWriter TextWriter)
		{
			this.TextWriter = TextWriter;
		}

		public override void close()
		{
			//throw new NotImplementedException();
		}

		public override void flush()
		{
			TextWriter.Flush();
			//throw new NotImplementedException();
		}

		public override void write(char[] charr, int index, int count)
		{
			TextWriter.Write(charr, index, count);
		}
	}

	static public class IKVMInterop
	{
		static public java.io.Reader GetJavaReader(this TextReader This)
		{
			return new IKVMReader(This);
		}

		static public java.io.Writer GetJavaWriter(this TextWriter This)
		{
			return new IKVMWriter(This);
		}
	}
}
