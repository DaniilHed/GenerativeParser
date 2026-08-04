

using FluentValidation;
using GenerativeParser.Features.ParseContent.DTOs;

namespace GenerativeParser.Features.ParseContent.Validation
{
	public class ParseContentRequestValidator: AbstractValidator<ParseContentRequest>
	{	
		public ParseContentRequestValidator()
		{
			RuleFor(x => x.Type)
				.IsInEnum()
				.WithMessage("Unsupported or missing 'type'.");

			RuleFor(x => x.Content)
				.NotEmpty()
				.WithMessage("Field 'content' cannot be empty.");
		}
	}
}
