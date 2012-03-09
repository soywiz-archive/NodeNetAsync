using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharpUtils.Templates.ParserNodes
{
	public class ParserNodeConstant : ParserNode
	{
		protected String Name;

		public ParserNodeConstant(String Name)
		{
			this.Name = Name;
		}

		override public void WriteTo(ParserNodeContext Context)
		{
			switch (Name)
			{
				case "true": Context.Write("true"); break;
				case "false": Context.Write("false"); break;
				case "none": Context.Write("null"); break;
				default: throw (new Exception(String.Format("Unknown constant '{0}'", Name)));
			}
		}

		public override string ToString()
		{
			return base.ToString() + "('" + Name + "')";
		}
	}
}
