using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUtils.Templates.ParserNodes
{
	public class ParserNodeContainer : ParserNode
	{
		protected List<ParserNode> Nodes;

		public override void Dump(int Level = 0, String Info = "")
		{
			base.Dump(Level, Info);
			int n = 0;
			foreach (var Node in Nodes)
			{
				Node.Dump(Level + 1, String.Format("Node{0}", n));
				n++;
			}
		}

		public ParserNodeContainer()
		{
			Nodes = new List<ParserNode>();
		}

		public void Add(ParserNode Node)
		{
			Nodes.Add(Node);
		}

		override public ParserNode Optimize(ParserNodeContext Context)
		{
			ParserNodeContainer OptimizedNode = CreateThisInstanceAs<ParserNodeContainer>();
			foreach (var Node in Nodes)
			{
				OptimizedNode.Add(Node.Optimize(Context));
			}
			return OptimizedNode;
		}

		override public void WriteTo(ParserNodeContext Context)
		{
			foreach (var Node in Nodes)
			{
				Node.WriteTo(Context);
			}
		}
	}
}
