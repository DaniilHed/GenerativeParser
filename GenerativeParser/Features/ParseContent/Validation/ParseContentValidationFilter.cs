using FluentValidation;
using Common.Errors;
using System.ComponentModel.DataAnnotations;

namespace GenerativeParser.Features.ParseContent.Validation
{
	public class ParseContentValidationFilter<T>(IValidator<T> validator) : IEndpointFilter where T : class
	{
		public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
		{
			var argument = context.Arguments.OfType<T>().FirstOrDefault();

			if (argument is null)
			{
				return Results.BadRequest(new Error(
					"Request.InvalidPayload",
					"The request body payload is missing or invalid."));
			}

			var validationResult = await validator.ValidateAsync(argument, context.HttpContext.RequestAborted);

			if (!validationResult.IsValid)
			{
				var errors = validationResult.Errors
					.GroupBy(e => e.PropertyName)
					.ToDictionary(
						g => g.Key,
						g => g.Select(e => e.ErrorMessage).ToArray()
					);

				return Results.ValidationProblem(errors);
			}

			return await next(context);
		}
	}
}
