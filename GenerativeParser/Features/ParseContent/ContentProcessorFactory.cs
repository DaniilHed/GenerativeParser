using GenerativeParser.Common;
using GenerativeParser.Common.Errors;
using GenerativeParser.Features.ParseContent.Enums;
using GenerativeParser.Features.ParseContent.Interfaces;
using System.Collections.ObjectModel;

namespace GenerativeParser.Features.ParseContent
{
	public class ContentProcessorFactory : IContentProcessorFactory
	{
		private readonly IReadOnlyDictionary<ContentType, IContentProcessor> _processors;

		public ContentProcessorFactory(IEnumerable<IContentProcessor> processors)
		{
			// Parsing registered processors
			_processors = new ReadOnlyDictionary<ContentType,IContentProcessor>((
				IDictionary<ContentType, IContentProcessor>)processors
				.ToDictionary(
					p => p.Type, p => p
				));
		}

		public Result<IContentProcessor>  GetProcessor(ContentType type)
		{
			if(_processors.TryGetValue(type,out var processor))
			{
				return Result.Success(processor);
			}

			return ParseContentErrors.UnsupportedType;

		}
	}
}
