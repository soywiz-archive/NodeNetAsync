using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUtils.Templates.ParserNodes
{
	public class ParserNodeAccess : ParserNode
	{
		ParserNode Parent;
		ParserNode Key;

		public ParserNodeAccess(ParserNode Parent, ParserNode Key)
		{
			this.Parent = Parent;
			this.Key = Key;
		}

		override public void WriteTo(ParserNodeContext Context)
		{
			Context.Write("DynamicUtils.Access(");
			Parent.WriteTo(Context);
			Context.Write(",");
			Key.WriteTo(Context);
			Context.Write(")");
		}
	}
}
