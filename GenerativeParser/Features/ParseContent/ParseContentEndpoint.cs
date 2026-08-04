using GenerativeParser.Common.Errors;
using GenerativeParser.Features.ParseContent.DTOs;
using GenerativeParser.Features.ParseContent.Interfaces;
using GenerativeParser.Features.ParseContent.Validation;

namespace GenerativeParser.Features.ParseContent
{
	// Endpoint for parcing content feature made with following Minimal API and Result patterns 
	public static class ParseContentEndpoint
	{
		public static void MapParseContentEndpoint (this IEndpointRouteBuilder app)
		{
			app.MapPost("/api/v1/parse-content", 
				(ParseContentRequest request, IParseContentHandler handler)
				=>
				{
					var result = handler.Handle(request);

					if (result.IsFailure)
					{
						return Results.BadRequest(result.Error);
					}

					return Results.Ok(result.Value);
				}
			)
				.WithName("ParseContent")
				.Accepts<ParseContentRequest>("application/json")
				.Produces<ParseContentResponse>(StatusCodes.Status200OK)
				.Produces<ParseContentResponse>(StatusCodes.Status400BadRequest)
				.ProducesValidationProblem(StatusCodes.Status400BadRequest)
				.AddEndpointFilter<ParseContentValidationFilter<ParseContentRequest>>(); ;
		}

	}
}
