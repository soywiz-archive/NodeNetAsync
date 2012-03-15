using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUtils.Templates.ParserNodes
{
	public class ParserNodeOutputExpression : ParserNodeParent
	{
		override public void WriteTo(ParserNodeContext Context)
		{
			Context.Write(Context._GetContextWriteAutoFilteredMethod() + "(");
			Parent.WriteTo(Context);
			Context.Write(");");
			Context.WriteLine("");
		}
	}
}
