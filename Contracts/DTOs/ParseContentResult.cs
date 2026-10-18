

namespace Contracts.DTOs
{
	public record ParseContentResult
	(
		int RecordCount,
		IReadOnlyCollection<ParsedItemDto> Data
	);
}
