using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CSharpUtils.Templates.TemplateProvider;
using CSharpUtils.Templates.Utils;
using CSharpUtils.Templates.Runtime;
using System.Reflection.Emit;
using System.Reflection;
using System.Threading.Tasks;

namespace CSharpUtils.Templates.ParserNodes
{
	abstract public class ParserNode
	{
		virtual public ParserNode Optimize(ParserNodeContext Context)
		{
			return this;
		}

		virtual public void Dump(int Level = 0, String Info = "")
		{
			Console.WriteLine("{0}{1}:{2}", new String(' ', Level * 4), Info, this);
		}

		virtual public void WriteTo(ParserNodeContext Context)
		{
		}

		virtual public void GenerateIL(ILGenerator ILGenerator, ParserNodeContext Context)
		{
			throw(new NotImplementedException());
		}

		protected T CreateThisInstanceAs<T>()
		{
			return (T)(Activator.CreateInstance(this.GetType()));
		}

		/*
		override public ParserNode Optimize(ParserNodeContext Context)
		{
			ParserNodeParent ParserNodeParent = Activator.CreateInstance(this.GetType());
			ParserNodeParent.Parent = Parent.Optimize(Context);
			return ParserNodeParent;
		}
		*/


		public override string ToString()
		{
			return String.Format("{0}", this.GetType().Name);
		}

		internal void OptimizeAndWrite(ParserNodeContext Context)
		{
			Optimize(Context).WriteTo(Context);
		}
	}

	public class DummyParserNode : ParserNode
	{
	}
}
