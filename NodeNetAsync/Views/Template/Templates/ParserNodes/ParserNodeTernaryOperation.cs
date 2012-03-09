using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUtils.Templates.ParserNodes
{
	public class ParserNodeTernaryOperation : ParserNode
	{
		public ParserNode ConditionNode;
		public ParserNode TrueNode;
		public ParserNode FalseNode;
		public String Operator;

		public ParserNodeTernaryOperation(ParserNode ConditionNode, ParserNode TrueNode, ParserNode FalseNode, String Operator)
		{
			this.ConditionNode = ConditionNode;
			this.TrueNode = TrueNode;
			this.FalseNode = FalseNode;
			this.Operator = Operator;
		}

		override public void WriteTo(ParserNodeContext Context)
		{
			switch (Operator)
			{
				case "?":
					Context.Write("DynamicUtils.ConvertToBool(");
					ConditionNode.WriteTo(Context);
					Context.Write(")");
					Context.Write("?");
					Context.Write("(");
					TrueNode.WriteTo(Context);
					Context.Write(")");
					Context.Write(":");
					Context.Write("(");
					FalseNode.WriteTo(Context);
					Context.Write(")");
					break;
				default:
					throw (new Exception(String.Format("Unknown Operator '{0}'", Operator)));
			}
		}
	}
}
