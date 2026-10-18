using Contracts.DTOs;
using Features.ParseContent.Interfaces;

using GenerativeParser.Features.ParseContent.Validation;
using MassTransit;

namespace GenerativeParser.Features.ParseContent
{
	// Endpoint for parcing content feature made with following Minimal API and Result patterns 
	public static class ParseContentEndpoint
	{
		public static void MapParseContentEndpoint (this IEndpointRouteBuilder app)
		{
			app.MapPost("/api/v1/parse-content", 
				async (ParseContentRequest request,
				IParseContentHandler handler,
				IPublishEndpoint publishEndpoint,
				CancellationToken cancellationToken) =>
				{
					var result = await handler.Handle(request, cancellationToken);

					if (result.IsFailure)
					{
						return Results.BadRequest(result.Error);
					}

					return Results.Accepted($"api/v1/jobs/{result.Value.JobId}",result.Value);
				}
			)
			.WithName("ParseContent")
			.Accepts<ParseContentRequest>("application/json")
			.Produces<ParseContentResult>(StatusCodes.Status202Accepted)
			.Produces<ParseContentResult>(StatusCodes.Status400BadRequest)
			.ProducesValidationProblem(StatusCodes.Status400BadRequest)
			.AddEndpointFilter<ParseContentValidationFilter<ParseContentRequest>>(); ;
		}

	}
}
