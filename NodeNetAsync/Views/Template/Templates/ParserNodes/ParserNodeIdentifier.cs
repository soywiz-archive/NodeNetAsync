using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpUtils.Templates.Utils;

namespace CSharpUtils.Templates.ParserNodes
{
	public class ParserNodeIdentifier : ParserNode
	{
		protected String Id;

		public ParserNodeIdentifier(String Id)
		{
			this.Id = Id;
		}

		override public void WriteTo(ParserNodeContext Context)
		{
			Context.Write("Context.GetVar({0})", StringUtils.EscapeString(Id));
		}

		public override string ToString()
		{
			return base.ToString() + "('" + Id + "')";
		}
	}
}
