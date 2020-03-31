using AutoMapper;
using Elsa.Activities;
using Elsa.Activities.Http.Activities;
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
			services.AddAutoMapper(opt => opt.Advanced.AllowAdditiveTypeMapCreation = true, typeof(Startup).Assembly);

			//services.AddWorkflowConfiguration(() => );

			services.AddElsa(elsa =>
			{
				elsa.AddEntityFrameworkStores<SqlServerContext>(options => options.UseSqlServer(Configuration.GetConnectionString("workflowDataSource")));
			}).AddHttpActivities()
			.AddElsaDashboard(
			options =>
			{
				options.Configure(x =>
				{
					/*
					 * If I do not add these two lines then I do not get the Activities registered on the Dashboard. See Ref #260 - https://github.com/elsa-workflows/elsa-core/issues/260
					 */
					x.ActivityDefinitions.Discover(t => t.FromAssembliesOf(typeof(SetVariable)));
					x.ActivityDefinitions.Discover(t => t.FromAssemblies(typeof(ReceiveHttpRequest).Assembly));
				});
			});


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