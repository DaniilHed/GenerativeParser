using GenerativeParser.Common;
using GenerativeParser.Features.ParseContent.Enums;

namespace GenerativeParser.Features.ParseContent.Interfaces
{
	public interface IContentProcessorFactory
	{
		Result<IContentProcessor> GetProcessor(ContentType type);
	}
}
