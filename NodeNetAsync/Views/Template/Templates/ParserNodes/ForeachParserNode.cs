﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpUtils.Templates.Utils;

namespace CSharpUtils.Templates.ParserNodes
{
	public class ForeachParserNode : ParserNode
	{
		public String VarName;
		public ParserNode LoopIterator;
		public ParserNode BodyBlock;
		public ParserNode ElseBlock;

		public override void Dump(int Level = 0, String Info = "")
		{
			base.Dump(Level, Info);
			LoopIterator.Dump(Level + 1, "LoopIterator");
			BodyBlock.Dump(Level + 1, "BodyBlock");
		}

		override public void WriteTo(ParserNodeContext Context)
		{
			//DynamicUtils.CountArray

			//Foreach(TemplateContext Context, String VarName, dynamic Expression, Action Iteration, Action Else = null)

			Context.WriteLine("await Context.NewScopeAsync(async delegate() {");
			Context.Indent(delegate()
			{
				Context.Write("await ForeachAsync(Context, {0}, ", StringUtils.EscapeString(VarName));
				Context.Indent(delegate()
				{
					LoopIterator.WriteTo(Context);
				});
				Context.Write(", ");
				Context.WriteLine("new EmptyDelegate(async delegate() {");
				Context.Indent(delegate()
				{
					BodyBlock.WriteTo(Context);
				});
				Context.Write("})");
				if (!(ElseBlock is DummyParserNode))
				{
					Context.Write(", ");
					Context.WriteLine("new EmptyDelegate(async delegate() {");
					ElseBlock.WriteTo(Context);
					Context.Write("})");
				}
				Context.WriteLine(");");  // ForeachAsync
			});
			Context.WriteLine("});"); // Context.NewScopeAsync
		}

		public override string ToString()
		{
			return base.ToString() + "('" + VarName + "')";
		}
	}
}
