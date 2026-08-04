using GenerativeParser.Common.Errors;

namespace GenerativeParser.Features.ParseContent.DTOs
{
	public record ParseContentResponse
	(
		int RecordCount,
		IReadOnlyCollection<ParsedItemDto> Data
	);
}
