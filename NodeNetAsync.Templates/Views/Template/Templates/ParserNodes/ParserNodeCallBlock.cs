using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpUtils.Templates.Utils;

namespace CSharpUtils.Templates.ParserNodes
{
	public class ParserNodeCallBlock : ParserNode
	{
		public String BlockName;

		public ParserNodeCallBlock(String BlockName)
		{
			this.BlockName = BlockName;
		}

		override public void WriteTo(ParserNodeContext Context)
		{
			Context.WriteLine("await CallBlockAsync({0}, Context);", StringUtils.EscapeString(this.BlockName));
			Context.WriteLine("");
		}
	}
}
