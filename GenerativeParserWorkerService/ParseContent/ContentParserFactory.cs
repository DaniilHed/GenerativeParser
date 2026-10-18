

using Common;
using Common.Errors;

using WorkerService.ParseContent.Interfaces;

namespace WorkerService.ParseContent
{
	public class ContentParserFactory(IEnumerable<IContentParser> parsers) : IContentParserFactory
	{
		private readonly Dictionary<string, IContentParser> _parsers= 
			parsers.ToDictionary(p=>p.ContentType, StringComparer.OrdinalIgnoreCase);

		public Result<IContentParser>  GetParser(string type)
		{
			if(_parsers.TryGetValue(type,out var parser))
			{
				return Result.Success(parser);
			}

			return ParseContentErrors.UnsupportedType;

		}
	}
}
