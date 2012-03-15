﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CSharpUtils.Templates.TemplateProvider;
using CSharpUtils.Html;
using CSharpUtils.Templates.Runtime.Filters;
using System.Reflection;
using System.Threading.Tasks;

namespace CSharpUtils.Templates.Runtime
{
	sealed public class TemplateContext
	{
		public TemplateCode RenderingTemplate;
		public TemplateFactory TemplateFactory;
		public TextWriter Output;
		public TemplateScope Scope;
		public dynamic Parameters;
		public bool Autoescape = true;
		public Dictionary<String, Tuple<Type, string>> Filters;

		public TemplateContext(TextWriter Output, TemplateFactory TemplateFactory, TemplateScope Scope = null)
		{
			if (Scope == null) Scope = new TemplateScope();

			this.Output = Output;
			this.Scope = Scope;
			this.TemplateFactory = TemplateFactory;

			Filters = new Dictionary<string, Tuple<Type, string>>();

			AddFilterLibrary(typeof(CoreFilters));
		}

		public void AddFilterLibrary(Type FilterLibraryType)
		{
			foreach (var Method in FilterLibraryType.GetMethods(BindingFlags.Static | BindingFlags.Public))
			{
				foreach (var Attribute in Method.GetCustomAttributes(typeof(TemplateFilterAttribute), true))
				{
					TemplateFilterAttribute TemplateFilterAttribute = (TemplateFilterAttribute)Attribute;
					this.AddFilter(TemplateFilterAttribute.Name, FilterLibraryType, Method.Name);
				}
			}
		}

		public void AddFilter(String FilterName, Type Type, String FunctionName)
		{
			Filters[FilterName] = new Tuple<Type,string>(Type, FunctionName);
		}

		public dynamic CallFilter(string FilterName, params dynamic[] Params)
		{
			Tuple<Type, string> Info;
			if (Filters.TryGetValue(FilterName, out Info))
			{
				return DynamicUtils.Call(Info.Item1, Info.Item2, Params);
			}
			else
			{
				return null;
			}
		}

		async public Task OutputWriteAutoFilteredAsync(dynamic Value)
		{
			if (Value != null)
			{
				await Output.WriteAsync("" + AutoFilter(Value));
			}
		}

		async public Task NewScopeAsync(Func<Task> Action)
		{
			this.Scope = new TemplateScope(this.Scope);
			await Action();
			this.Scope = this.Scope.ParentScope;
		}

		public dynamic GetVar(String Name)
		{
			try
			{
				return Scope[Name];
			}
			catch (Exception)
			{
				return null;
			}
		}

		public void SetVar(String Name, dynamic Value)
		{
			Scope[Name] = Value;
		}

		public dynamic AutoFilter(dynamic Value)
		{
			if (Value is RawWrapper || !Autoescape)
			{
				return Value;
			}
			return HtmlUtils.EscapeHtmlCharacters(DynamicUtils.ConvertToString(Value));
		}
	}
}