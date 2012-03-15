using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CSharpUtils.Templates.TemplateProvider
{
	public class TemplateProviderMemory : ITemplateProvider
    {
        Dictionary<String, byte[]> Map;

        public TemplateProviderMemory()
        {
            Map = new Dictionary<string, byte[]>();
        }

        public void Add(String Name, String Data, Encoding Encoding = null)
        {
            if (Encoding == null) Encoding = Encoding.UTF8;
			Map[Name] = Encoding.GetBytes(Data);
        }

        public Stream GetTemplate(string Name)
        {
            if (!Map.ContainsKey(Name)) throw(new Exception(String.Format("Not Mapped File '{0}'", Name)));
            return new MemoryStream(Map[Name]);
        }
    }
}
