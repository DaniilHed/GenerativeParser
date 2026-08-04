using GenerativeParser.Common;
using GenerativeParser.Features.ParseContent.DTOs;
using GenerativeParser.Features.ParseContent.Enums;


namespace GenerativeParser.Features.ParseContent.Interfaces
{
	public interface IContentProcessor
	{
		ContentType Type { get; }
		Result<IReadOnlyCollection<ParsedItemDto>> Process(string content);
	}
}
