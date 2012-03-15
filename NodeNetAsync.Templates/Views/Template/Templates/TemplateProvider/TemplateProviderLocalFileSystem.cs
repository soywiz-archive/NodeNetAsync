using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpUtils.Templates.TemplateProvider;
using NodeNetAsync.Utils;

namespace CSharpUtils.Templates.Templates.TemplateProvider
{
	public class TemplateProviderLocalFileSystem : ITemplateProvider
	{
		String RootPath;

		public TemplateProviderLocalFileSystem(String RootPath)
		{
			this.RootPath = RootPath;
		}

		public Stream GetTemplate(string Name)
		{
			return File.Open(Url.GetInnerFileRelativeToPath(this.RootPath, Name), FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
		}
	}
}
