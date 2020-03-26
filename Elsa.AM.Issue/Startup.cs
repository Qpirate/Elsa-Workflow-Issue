using AutoMapper;
using Elsa.AM.WorkflowCodebase.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using IConfigurationProvider = AutoMapper.IConfigurationProvider;

namespace Elsa.AM.Issue
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		public void ConfigureServices(IServiceCollection services)
		{
			/*
			* 
			* SCENARIO 2 - Automapper added after call to create Elsa Workflow Items
			* 
			*/
			services.AddWorkflowConfiguration(() => Configuration.GetConnectionString("workflowDataSource"));
			services.AddAutoMapper(typeof(EM.AM.Issue.Profiles.ClassA).Assembly);
			/* 
			 * OUTCOME: Does Not work	   
			 */

			/*
			* 
			* SCENARIO 2 - Automapper added after call to create Elsa Workflow Items, with Advanced Merge Map enabled
			* 
			*/
			//services.AddWorkflowConfiguration(() => Configuration.GetConnectionString("workflowDataSource"));
			//services.AddAutoMapper(mcExp =>mcExp.Advanced.AllowAdditiveTypeMapCreation= true, typeof(EM.AM.Issue.Profiles.ClassA).Assembly);
			/* 
			 * OUTCOME: Does Not work	   
			 */

			/*
			 * 
			 * SCENARIO 3 - Automapper added before call to create Elsa Workflow Items. 
			 * 
			 */
			//services.AddAutoMapper(typeof(EM.AM.Issue.Profiles.ClassA).Assembly);
			//services.AddWorkflowConfiguration(() => Configuration.GetConnectionString("workflowDataSource"));
			/* 
			 * OUTCOME: Works correctly with no modification
			*/

			services.AddMvc(config => config.EnableEndpointRouting = false)
			.AddNewtonsoftJson(a => a.SerializerSettings.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter(new Newtonsoft.Json.Serialization.CamelCaseNamingStrategy(), true)));
		}

		public static void Configure(IApplicationBuilder app, IWebHostEnvironment env, IConfigurationProvider autoMapper)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				autoMapper.AssertConfigurationIsValid();
			}

			app.UseHttpsRedirection();

#pragma warning disable MVC1005 // Cannot use UseMvc with Endpoint Routing.
			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller}/{action=Index}/{id?}");
			});
#pragma warning restore MVC1005 // Cannot use UseMvc with Endpoint Routing.
		}
	}
}