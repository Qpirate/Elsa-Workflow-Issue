using Elsa.Activities;
using Elsa.Activities.Http.Activities;
using Elsa.Activities.Http.Extensions;
using Elsa.Dashboard.Extensions;
using Elsa.Persistence.EntityFrameworkCore.DbContexts;
using Elsa.Persistence.EntityFrameworkCore.Extensions;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Elsa.AM.WorkflowCodebase.DependencyInjection
{
	public static class WorkflowDI
	{
		public static IServiceCollection AddWorkflowConfiguration(this IServiceCollection services, Func<string> connectionString)
		{
			services.AddElsa(elsa =>
			{
				elsa.AddEntityFrameworkStores<SqlServerContext>(options => options.UseSqlServer(connectionString.Invoke()));
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

			return services;
		}

		public static void UseWorkflowConfiguration(this IApplicationBuilder app)
		{
			app.UseHttpActivities();

			using var scope = app.ApplicationServices.CreateScope();
			var scopeProvider = scope.ServiceProvider;
			var context = scopeProvider.GetRequiredService<ElsaContext>();

			context.Database.Migrate();
		}
	}
}
