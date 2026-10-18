using Contracts.DTOs;
using Features.ParseContent.Interfaces;
using Common;
using MassTransit;
using Infrastructure.ParseContent.DataBaseContext;
using Infrastructure.ParseContent.Entities;
using Common.Errors;
using Microsoft.EntityFrameworkCore;



namespace GenerativeParser.Features.ParseContent
{
	// Hadnler that uses specialised classes to handle parsing and return coorrect response
	public class ParseContentHandler(
		ILogger<ParseContentHandler> logger,
		ParseContentDbContext dbContext,
		IPublishEndpoint publishEndpoint
		) : IParseContentHandler
	{
		public async Task<Result<ParseContentResponse>> Handle(ParseContentRequest request, CancellationToken cancellationToken = default)
		{
			logger.LogInformation("Creating job for ContentType: {ContentType}", request.ContentType);


			var job = ParseContentJob.Create(request.ContentType, request.Content);
			try
			{
				 dbContext.Add(job);
				await dbContext.SaveChangesAsync(cancellationToken);
			}
			catch (DbUpdateException ex) 
			{
				logger.LogError("Saving job with {JobId} to data base Canceled", job.Id);
				return new Error("Database.SaveFailed", $"Database error occurred while saving the job with Id: {job.Id}.");
			}

			await publishEndpoint.Publish<ParseContentCommand>(new ParseContentCommand(job.Id),cancellationToken);


			return new ParseContentResponse(job.Id,job.Status.ToString());
		}
	}
}
