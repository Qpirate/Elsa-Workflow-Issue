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
			services.AddAutoMapper(typeof(EM.AM.Issue.Profiles.ClassA).Assembly);

			services.AddWorkflowConfiguration(() => Configuration.GetConnectionString("workflowDataSource"));
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
			app.UseWorkflowConfiguration();

			app.UseStaticFiles()
			.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "default",
					template: "{controller}/{action=Index}/{id?}");
			});
		}
	}
}