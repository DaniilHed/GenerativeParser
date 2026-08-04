using GenerativeParser.Features.ParseContent.Enums;

namespace GenerativeParser.Features.ParseContent.DTOs
{
	public record ParseContentRequest
	(
		ContentType Type,
		string Content
	);
}
