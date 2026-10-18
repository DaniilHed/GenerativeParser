namespace Contracts.DTOs
{
	public sealed record ParsedItemDto
	(
		IReadOnlyDictionary<string,object?> Values
	);
}
