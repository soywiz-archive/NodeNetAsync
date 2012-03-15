using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUtils.Templates.ParserNodes
{
	public class ParserNodeParent : ParserNode
	{
		public ParserNode Parent;

		public override void Dump(int Level = 0, String Info = "")
		{
			base.Dump(Level, Info);
			Parent.Dump(Level + 1, "Parent");
		}

		override public ParserNode Optimize(ParserNodeContext Context)
		{
			var That = CreateThisInstanceAs<ParserNodeParent>();
			That.Parent = Parent.Optimize(Context);
			return That;
		}
	}
}
