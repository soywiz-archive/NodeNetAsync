using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpUtils.Templates.TemplateProvider;
using NodeNetAsync.Utils;
using NodeNetAsync.Vfs;

namespace CSharpUtils.Templates.Templates.TemplateProvider
{
	public class TemplateProviderVirtualFileSystem : ITemplateProvider
	{
		IVirtualFileSystem VirtualFileSystem;

		public TemplateProviderVirtualFileSystem(IVirtualFileSystem VirtualFileSystem)
		{
			this.VirtualFileSystem = VirtualFileSystem;
		}

		async public Task<Stream> GetTemplateAsync(string Name)
		{
			return (await VirtualFileSystem.OpenAsync(Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)).SystemStream;
		}
	}
}
