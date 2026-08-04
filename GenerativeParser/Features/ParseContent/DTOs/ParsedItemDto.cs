namespace GenerativeParser.Features.ParseContent.DTOs
{
	public sealed record ParsedItemDto
	(
		IReadOnlyDictionary<string,object?> Values
	);
}
