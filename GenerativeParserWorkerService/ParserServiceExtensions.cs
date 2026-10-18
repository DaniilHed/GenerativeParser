using WorkerService.ParseContent.Interfaces;
using WorkerService.ParseContent.Processors;

namespace WorkerService
{
	public static class ParserServiceExtensions
	{
		public static IServiceCollection ConfigureParsers (this IServiceCollection services)
		{
			services.AddTransient<IContentParser, InternalJsonContentParser>();
			services.AddTransient<IContentParser, CsvContentParser>();
			return services;
		}
	}
}
