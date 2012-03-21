using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CSharpUtils.Templates.TemplateProvider;
using CSharpUtils.Templates.Runtime;
using CSharpUtils.Templates.Tokenizers;
using CSharpUtils.Templates.Templates;
using System.Threading.Tasks;
using NodeNetAsync.Utils;

namespace CSharpUtils.Templates
{
	public class TemplateFactory
	{
		protected Encoding Encoding;
		protected ITemplateProvider TemplateProvider;
		protected AsyncCache<String, Type> AsyncCachedTemplatesByFile = new AsyncCache<string, Type>();
		protected bool OutputGeneratedCode;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="TemplateProvider"></param>
		/// <param name="Encoding"></param>
		/// <param name="OutputGeneratedCode"></param>
		public TemplateFactory(ITemplateProvider TemplateProvider, Encoding Encoding, bool OutputGeneratedCode = false)
		{
			this.Encoding = Encoding;
			this.TemplateProvider = TemplateProvider;
			this.OutputGeneratedCode = OutputGeneratedCode;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="TemplateString"></param>
		/// <returns></returns>
		async protected Task<Type> GetTemplateCodeTypeByStringAsync(String TemplateString)
		{
			var TemplateCodeGen = new TemplateCodeGen(TemplateString, this);
			TemplateCodeGen.OutputGeneratedCode = OutputGeneratedCode;

			Exception YieldedException = null;

			var Value = await Task.Run(() =>
			{
				try
				{
					return TemplateCodeGen.GetTemplateCodeType();
				}
				catch (Exception Exception)
				{
					YieldedException = Exception;
					return null;
				}
			});

			if (YieldedException != null) throw YieldedException;

			return Value;
			//return new TemplateCodeGenRoslyn(TemplateString, this).GetTemplateCodeType();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Name"></param>
		/// <returns></returns>
		async protected Task<Type> GetTemplateCodeTypeByFileAsync(String Name)
		{
			if (TemplateProvider == null) throw(new Exception("No specified TemplateProvider"));

			return await AsyncCachedTemplatesByFile.GetAsync(Name, async () =>
			{
				Console.WriteLine("Generating code for template: '{0}'", Name);
				using (var TemplateStream = await TemplateProvider.GetTemplateAsync(Name))
				{
					var TemplateData = new byte[TemplateStream.Length];
					await TemplateStream.ReadAsync(TemplateData, 0, (int)TemplateStream.Length);
					return await GetTemplateCodeTypeByStringAsync(Encoding.GetString(TemplateData));
				}
			});
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Type"></param>
		/// <returns></returns>
		protected TemplateCode CreateInstanceByType(Type Type)
		{
			return (TemplateCode)Activator.CreateInstance(Type, this);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="TemplateString"></param>
		/// <returns></returns>
		async public Task<TemplateCode> GetTemplateCodeByStringAsync(String TemplateString)
		{
			return CreateInstanceByType(await GetTemplateCodeTypeByStringAsync(TemplateString));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="Name"></param>
		/// <returns></returns>
		async public Task<TemplateCode> GetTemplateCodeByFileAsync(String Name)
		{
			return CreateInstanceByType(await GetTemplateCodeTypeByFileAsync(Name));
		}
	}
}
