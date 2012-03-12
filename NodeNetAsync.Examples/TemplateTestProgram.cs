using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSharpUtils.Templates;
using CSharpUtils.Templates.Runtime;
using CSharpUtils.Templates.TemplateProvider;
using NodeNetAsync.Net.Http;
using NodeNetAsync.Net.Http.Router;
using NodeNetAsync.Net.Https;
using NodeNetAsync.Views;

namespace NodeNetAsync.Examples
{
	public class TemplateTestProgram
	{
		public class Item
		{
			/// <summary>
			/// Name of the item
			/// </summary>
			public string Name;

			/// <summary>
			/// Age of the item
			/// </summary>
			public int Age;
		}

		static public void Main(string[] Args)
		{
			Core.Loop(async () => 
			{
				var HttpServer = new HttpServer();
				var HttpRouter = new HttpRouter();

				// Creates a TemplateRendered that will read templates from memory (specified with Add method).
				var MyTemplateRenderer = await TemplateRenderer.CreateFromMemoryAsync(OutputGeneratedCode: true);

				// Adds a layout template
				MyTemplateRenderer.Add("_layout", @"
					<html>
						<head>
							<title>Title</title>
						</head>
						<body>
							<div>{% block Header %}Header{% endblock %}</div>
							<div>{% block Content %}Content{% endblock %}</div>
							<div>{% block Footer %}Footer{% endblock %}</div>
						</body>
					</html>
				".Trim());

				// Adds a template that extends _layout.
				MyTemplateRenderer.Add("test", @"
					{% extends '_layout' %}
					{% block Content %}
						<h1>Item List</h1>
						<ul>
						{% for Item in List %}
							<li>Name: {{ Item.Name }}, Age: {{ Item.Age }}</li>
						{% endfor %}
						</ul>
					{% endblock %}
				".Trim());

				HttpRouter.AddRoute("/", async (Request, Response) =>
				{
					Response.Buffering = true;

					await MyTemplateRenderer.WriteToAsync(
						Stream: Response,
						TemplateName: "test",
						Scope: new Dictionary<string, object>() 
						{
							{
								"List", new[] {
									new Item() { Name = "Hello", Age = 3 },
									new Item() { Name = "World", Age = 17 },
									new Item() { Name = "This", Age = 999 },
									new Item() { Name = "Is", Age = -1 },
									new Item() { Name = "A", Age = 0 },
									new Item() { Name = "Test", Age = 33 },
								}
							},
						}
					);
				});

				HttpServer.AddFilterLast(HttpRouter);
				await HttpServer.ListenAsync(80, "127.0.0.1");
			});
		}
	}
}
