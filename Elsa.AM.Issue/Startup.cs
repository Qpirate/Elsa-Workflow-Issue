using AutoMapper;
using Elsa.Activities.Http.Extensions;
using Elsa.Dashboard.Extensions;
using Elsa.Persistence.EntityFrameworkCore.DbContexts;
using Elsa.Persistence.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
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
			services.AddAutoMapper(opt => opt.Advanced.AllowAdditiveTypeMapCreation = true, typeof(Startup).Assembly, typeof(Elsa.Models.ActivityDefinition).Assembly, typeof(Elsa.Persistence.EntityFrameworkCore.Mapping.EntitiesProfile).Assembly);

			services.AddElsa(elsa =>
			{
				elsa.AddEntityFrameworkStores<SqlServerContext>(options => options.UseSqlServer(Configuration.GetConnectionString("workflowDataSource")));
			}).AddHttpActivities()
			.AddElsaDashboard();

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

			app.UseHttpActivities();

			using var scope = app.ApplicationServices.CreateScope();
			var scopeProvider = scope.ServiceProvider;
			var context = scopeProvider.GetRequiredService<ElsaContext>();

			context.Database.Migrate();

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