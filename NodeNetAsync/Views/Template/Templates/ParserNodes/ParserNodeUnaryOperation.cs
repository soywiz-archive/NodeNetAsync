using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUtils.Templates.ParserNodes
{
	public class ParserNodeUnaryOperation : ParserNodeParent
	{
		public String Operator;

		override public void WriteTo(ParserNodeContext Context)
		{
			Context.Write("{0}(", Operator);
			Parent.WriteTo(Context);
			Context.Write(")");
		}
	}
}
