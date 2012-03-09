using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Dynamic;
using System.Reflection;
using CSharpUtils.Templates.TemplateProvider;
using System.Threading.Tasks;

namespace CSharpUtils.Templates.Runtime
{
	internal class DummyTemplateCode : TemplateCode
	{
		protected override Task LocalRenderAsync(TemplateContext Context)
		{
 			throw new NotImplementedException();
		}
	}
	abstract public class TemplateCode
	{
		static public readonly TemplateCode Methods = new DummyTemplateCode();

		TemplateFactory TemplateFactory;
		public delegate Task RenderDelegate(TemplateContext Context);
		Dictionary<String, RenderDelegate> Blocks = new Dictionary<string, RenderDelegate>();
		TemplateCode ChildTemplate;
		TemplateCode ParentTemplate;

		public TemplateCode(TemplateFactory TemplateFactory = null)
		{
			this.TemplateFactory = TemplateFactory;
			this.Init();
		}

		public void Init()
		{
			this.SetBlocks(this.Blocks);
		}

		virtual public void SetBlocks(Dictionary<String, RenderDelegate> Blocks)
		{
		}

		protected void SetBlock(Dictionary<String, RenderDelegate> Blocks, String BlockName, RenderDelegate Callback)
		{
			Blocks[BlockName] = Callback;
		}

		abstract protected Task LocalRenderAsync(TemplateContext Context);

		/*
		async virtual protected Task LocalRenderAsync(TemplateContext Context)
		{
		}
		*/

		/*
		async public Task RenderAsync(TemplateContext Context)
		{
		}
		*/

		async public Task RenderAsync(TemplateContext Context)
		{
			Context.RenderingTemplate = this;

			Exception ProducedException = null;
			try
			{
				await this.LocalRenderAsync(Context);
			}
			catch (FinalizeRenderException)
			{
			}
			catch (Exception Exception)
			{
				ProducedException = Exception;
			}
			if (ProducedException != null)
			{
				await Context.Output.WriteLineAsync(ProducedException.ToString());
				//throw (ProducedException);
			}
		}

		public String RenderToString(TemplateScope Scope = null)
		{
			if (Scope == null) Scope = new TemplateScope();
			var StringWriter = new StringWriter();
			RenderAsync(new TemplateContext(StringWriter, TemplateFactory, Scope)).Wait();
			return StringWriter.ToString();
		}

		async protected Task SetAndRenderParentTemplateAsync(String ParentTemplateFileName, TemplateContext Context)
		{
			this.ParentTemplate = await Context.TemplateFactory.GetTemplateCodeByFileAsync(ParentTemplateFileName);
			this.ParentTemplate.ChildTemplate = this;
			await this.ParentTemplate.LocalRenderAsync(Context);

			throw (new FinalizeRenderException());
		}

		async protected Task CallBlockAsync(String BlockName, TemplateContext Context)
		{
			await Context.RenderingTemplate.GetFirstAscendingBlock(BlockName)(Context);
		}

		protected RenderDelegate GetFirstAscendingBlock(String BlockName)
		{
			if (this.Blocks.ContainsKey(BlockName))
			{
				return this.Blocks[BlockName];
			}

			if (this.ParentTemplate != null)
			{
				return this.ParentTemplate.GetFirstAscendingBlock(BlockName);
			}
			
			throw(new Exception(String.Format("Can't find ascending parent block '{0}'", BlockName)));
		}

		protected void CallParentBlock(String BlockName, TemplateContext Context)
		{
			this.ParentTemplate.GetFirstAscendingBlock(BlockName)(Context);
		}

		public delegate Task EmptyDelegate();

		protected void Autoescape(TemplateContext Context, dynamic Expression, EmptyDelegate Block)
		{
			bool OldAutoescape = Context.Autoescape;
			Context.Autoescape = Expression;
			{
				Block();
			}
			Context.Autoescape = OldAutoescape;
		}

		async protected Task ForeachAsync(TemplateContext Context, String VarName, dynamic Expression, EmptyDelegate Iteration, EmptyDelegate Else = null)
		{
			int Index = 0;
			foreach (var Item in DynamicUtils.ConvertToIEnumerable(Expression))
			{
				Context.SetVar("loop", new Dictionary<String, dynamic> {
					{ "index", Index + 1 },
					{ "index0", Index },
				});
				Context.SetVar(VarName, Item);
				await Iteration();
				Index++;
			}

			if (Index == 0)
			{
				if (Else != null) await Else();
			}
		}
	}
}
