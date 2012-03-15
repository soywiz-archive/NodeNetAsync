using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUtils.Templates.ParserNodes
{
	public class ParserNodeIf : ParserNode
	{
		public ParserNode ConditionNode;
		public ParserNode BodyIfNode;
		public ParserNode BodyElseNode;

		public override void Dump(int Level = 0, String Info = "")
		{
			base.Dump(Level, Info);
			ConditionNode.Dump(Level + 1, "Condition");
			BodyIfNode.Dump(Level + 1, "IfBody");
			BodyElseNode.Dump(Level + 1, "ElseBody");
		}

		override public void WriteTo(ParserNodeContext Context)
		{
			Context.Write("if (DynamicUtils.ConvertToBool(");
			ConditionNode.WriteTo(Context);
			Context.Write(")) {");
			Context.WriteLine("");
			BodyIfNode.WriteTo(Context);
			Context.Write("}");
			if (!(BodyElseNode is DummyParserNode))
			{
				Context.Write(" else {");
				BodyElseNode.WriteTo(Context);
				Context.Write("}");
			}
			Context.WriteLine("");
		}
	}
}
