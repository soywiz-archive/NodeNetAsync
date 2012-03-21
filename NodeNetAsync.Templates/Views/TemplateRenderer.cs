using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using CSharpUtils.Templates;
using CSharpUtils.Templates.Runtime;
using CSharpUtils.Templates.TemplateProvider;
using CSharpUtils.Templates.Templates.TemplateProvider;
using NodeNetAsync.Streams;
using NodeNetAsync.Utils;
using NodeNetAsync.Vfs;

namespace NodeNetAsync.Views
{
	public class VirtualFileSystemTemplateRenderer : TemplateRenderer
	{
		public VirtualFileSystemTemplateRenderer(IVirtualFileSystem FileSystem, bool OutputGeneratedCode)
		{
			this.TemplateFactory = new TemplateFactory(
				new TemplateProviderVirtualFileSystem(FileSystem),
				Encoding: Encoding.UTF8,
				OutputGeneratedCode: OutputGeneratedCode
			);
		}
	}

	abstract public class TemplateRenderer
	{
		protected TemplateFactory TemplateFactory;

		async static public Task<VirtualFileSystemTemplateRenderer> CreateFromVirtualFileSystemAsync(IVirtualFileSystem FileSystem, bool OutputGeneratedCode = false)
		{
			await Task.Yield();
			return new VirtualFileSystemTemplateRenderer(FileSystem, OutputGeneratedCode);
		}
		async protected Task<TemplateCode> GetTemplateCodeByFileAsync(string TemplateName)
		{
			return await TemplateFactory.GetTemplateCodeByFileAsync(TemplateName);
		}

		async public Task WriteToAsync(IAsyncWriter Stream, String TemplateName, Dictionary<string, object> Scope = null)
		{
			var TemplateCode = await GetTemplateCodeByFileAsync(TemplateName);
			var TextWriter = new NodeTextWriter(Stream);
			var TemplateContext = new TemplateContext(TextWriter, TemplateFactory, new TemplateScope(Scope));
			await TemplateCode.RenderAsync(TemplateContext);
			await TextWriter.FlushAsync();
		}

		public class NodeTextWriter : TextWriter
		{
			IAsyncWriter AsyncWriter;

			public NodeTextWriter(IAsyncWriter AsyncWriter)
			{
				this.AsyncWriter = AsyncWriter;
			}

			public override Encoding Encoding
			{
				get { return Encoding.UTF8; }
			}

			async public override Task WriteAsync(char[] buffer, int index, int count)
			{
				await AsyncWriter.WriteAsync(Encoding.GetBytes(buffer, index, count));
			}

			async public override Task WriteAsync(string value)
			{
				await AsyncWriter.WriteAsync(Encoding.GetBytes(value));
			}
		}
	}
}
