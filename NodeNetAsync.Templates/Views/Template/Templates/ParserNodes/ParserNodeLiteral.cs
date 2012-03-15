using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using CSharpUtils.Templates.Utils;

namespace CSharpUtils.Templates.ParserNodes
{
	public class ParserNodeNumericLiteral : ParserNode
	{
		public long Value;

		override public void WriteTo(ParserNodeContext Context)
		{
			Context.Write("{0}", Value);
		}

		override public void GenerateIL(ILGenerator ILGenerator, ParserNodeContext Context)
		{
			ILGenerator.Emit(OpCodes.Ldc_I8, Value);
		}

		public override string ToString()
		{
			return base.ToString() + "(" + Value + ")";
		}
	}

	public class ParserNodeStringLiteral : ParserNode
	{
		protected String Value;

		public ParserNodeStringLiteral(String Value)
		{
			this.Value = Value;
		}

		override public void WriteTo(ParserNodeContext Context)
		{
			Context.Write(StringUtils.EscapeString(Value));
		}

		override public void GenerateIL(ILGenerator ILGenerator, ParserNodeContext Context)
		{
			ILGenerator.Emit(OpCodes.Ldstr, Value);
		}

		public override string ToString()
		{
			return base.ToString() + "('" + Value + "')";
		}
	}

	public class ParserNodeLiteral : ParserNode
	{
		public String Text;

		override public void WriteTo(ParserNodeContext Context)
		{
			//Context.Write("Context.Output.Write(Context.AutoFilter({0}));", StringUtils.EscapeString(Text));
			Context.Write("{0}({1});", Context._GetContextWriteMethod(), StringUtils.EscapeString(Text));
			Context.WriteLine("");
		}

		/*
		override public void GenerateIL(ILGenerator ILGenerator, ParserNodeContext Context)
		{
			//ILGenerator.Emit(OpCodes.Call, typeof(StringUtils).GetMethod("EscapeString"));
		}
		*/

		public override string ToString()
		{
			return base.ToString() + "('" + Text + "')";
		}
	}
}
