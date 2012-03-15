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

namespace NodeNetAsync.Views
{
	public class FileSystemTemplateRenderer : TemplateRenderer
	{
		public FileSystemTemplateRenderer(string TemplateFolder, bool OutputGeneratedCode)
		{
			this.TemplateFactory = new TemplateFactory(
				new TemplateProviderLocalFileSystem(TemplateFolder),
				Encoding: Encoding.UTF8,
				OutputGeneratedCode: OutputGeneratedCode
			);
		}
	}

	public class MemoryTemplateRenderer : TemplateRenderer
	{
		TemplateProviderMemory TemplateProviderMemory;

		public MemoryTemplateRenderer(bool OutputGeneratedCode)
		{
			this.TemplateFactory = new TemplateFactory(
				this.TemplateProviderMemory = new TemplateProviderMemory(),
				Encoding: Encoding.UTF8,
				OutputGeneratedCode: OutputGeneratedCode
			);
		}

		public void Add(string Name, string Contents)
		{
			TemplateProviderMemory.Add(Name, Contents);
		}
	}

	abstract public class TemplateRenderer
	{
		protected TemplateFactory TemplateFactory;

		async static public Task<FileSystemTemplateRenderer> CreateFromFileSystemAsync(string TemplateFolder, bool OutputGeneratedCode = false)
		{
			await Task.Yield();
			return new FileSystemTemplateRenderer(TemplateFolder, OutputGeneratedCode);
		}

		async static public Task<MemoryTemplateRenderer> CreateFromMemoryAsync(bool OutputGeneratedCode = false)
		{
			await Task.Yield();
			return new MemoryTemplateRenderer(OutputGeneratedCode);
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
