using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpUtils.Templates.Utils;

namespace CSharpUtils.Templates.ParserNodes
{
	public class ParserNodeBlockParent : ParserNode
	{
		String BlockName;

		public ParserNodeBlockParent(String BlockName)
		{
			this.BlockName = BlockName;
		}

		override public void WriteTo(ParserNodeContext Context)
		{
			Context.WriteLine("CallParentBlock({0}, Context);", StringUtils.EscapeString(BlockName));
		}
	}
}
