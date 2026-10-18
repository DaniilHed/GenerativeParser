using Microsoft.AspNetCore.Mvc;


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
				await HandleExceptionAsync(context,ex);
			}
		}

		// Configures the response headers and writes a JSON ProblemDetails body for server errors
		private static Task HandleExceptionAsync(HttpContext context, Exception exception)
		{
			context.Response.ContentType = "application/json";
			context.Response.StatusCode = StatusCodes.Status500InternalServerError;

			var problemDetails = new ProblemDetails
			{
				Status = StatusCodes.Status500InternalServerError,
				Title = "Server Error",
				Detail = "An internal server error occurred. Please try again later.",
				Instance = context.Request.Path
			};

			return context.Response.WriteAsJsonAsync(problemDetails);
		}
	}
}
