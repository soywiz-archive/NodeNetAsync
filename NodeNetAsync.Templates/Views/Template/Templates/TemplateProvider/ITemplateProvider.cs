using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace CSharpUtils.Templates.TemplateProvider
{
	public interface ITemplateProvider
	{
		Task<Stream> GetTemplateAsync(String Name);
	}
}
