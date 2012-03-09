using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpUtils.Templates.Runtime;

namespace CSharpUtils.Templates.ParserNodes
{
	public class ParserNodeExtends : ParserNodeParent
	{
		override public void WriteTo(ParserNodeContext Context)
		{
			//Context.Write("await " + ((Func<string, TemplateContext, Task>)TemplateCode.Methods.SetAndRenderParentTemplateAsync).Method.Name + "(");
			Context.Write("await SetAndRenderParentTemplateAsync(");
			Parent.WriteTo(Context);
			Context.Write(", Context);");
			Context.WriteLine("");
		}
	}
}
