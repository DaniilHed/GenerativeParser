using Infrastructure.ParseContent.DataBaseContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<ParseContentDbContext>(options =>
				options.UseNpgsql(configuration.GetConnectionString("DefaultConnection"))
			);
			return services;
		}
	}
}
