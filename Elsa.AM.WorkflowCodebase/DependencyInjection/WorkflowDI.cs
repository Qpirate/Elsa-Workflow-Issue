using Elsa.Dashboard.Extensions;
using Elsa.Persistence.EntityFrameworkCore.DbContexts;
using Elsa.Persistence.EntityFrameworkCore.Extensions;
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
				elsa.AddEntityFrameworkStores<SqlServerContext>(options => options.UseSqlServer(connectionString.Invoke()))
			)
			.AddElsaDashboard();
			return services;
		}
	}
}
