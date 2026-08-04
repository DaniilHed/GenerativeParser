using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace GenerativeParser.Middleware
{
	// Catches unhandled exceptions during request processing and returns a standardized JSON response
	public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
	{
		// Executes the middleware in the HTTP request pipeline
		public async Task InvokeAsync(HttpContext context)
		{
			try
			{
				await next(context);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "An unhandled exception occurred: {Message}", ex.Message);
				await HandleExceptionAsync(context);
			}
		}

		// Configures the response headers and writes a JSON ProblemDetails body for server errors
		private static Task HandleExceptionAsync(HttpContext context)
		{
			// Set HTTP status code to 500 Internal Server Error
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

			// Create a generic error response to prevent data leaks
			var problemDetails = new ProblemDetails
			{
				Status = (int)HttpStatusCode.InternalServerError,
				Title = "Server Error",
				Detail = "An internal server error occurred. Please try again later."
			};

			return context.Response.WriteAsJsonAsync(problemDetails);
		}
	}
}
