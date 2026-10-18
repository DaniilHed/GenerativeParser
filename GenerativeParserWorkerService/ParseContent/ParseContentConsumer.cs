using Contracts.DTOs;
using Infrastructure.ParseContent.DataBaseContext;
using Infrastructure.ParseContent.Entities;
using Infrastructure.ParseContent.Enums;
using MassTransit;
using Microsoft.AspNetCore.SignalR;
using WorkerService.Hubs;
using WorkerService.ParseContent.Helpers;
using WorkerService.ParseContent.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;


namespace WorkerService.ParseContent
{
	public class ParseContentConsumer(
		ParseContentDbContext dbContext,
		IHubContext<ParseContentHub>  hubContext,
		IBase64Decoder base64Decoder,
		IContentParserFactory processorFactory,
		ILogger<ParseContentConsumer> logger,
		JobResultHandler resultHandler
		) : IConsumer<ParseContentCommand>
	{
		public async Task Consume(ConsumeContext<ParseContentCommand> context)
		{
			var jobId= context.Message.JobId;
			logger.LogInformation("Processing parse content job with Id: {JobId}",jobId);
			
			ParseContentJob? job= await dbContext.ParseJobs.FindAsync([jobId],context.CancellationToken);

			if (job is null) {
				logger.LogWarning("Data base could not find job with Id: {JobId}",jobId);
				return;
			}
			
			if (job.Status != JobStatus.Pending)
			{
				logger.LogWarning("Job Status must be Pending to start proccessing! Current job status with Id:{JobId} has Status: {JobStatus}", jobId, job.Status);
				return;
			}

			job.SetProcessing();

			await hubContext.SendJobStatusUpdateAsync(jobId, job.Status);

			// Decode base64 content with custom decoder class
			var decodedResult = base64Decoder.Decode(job.Content);
			if (decodedResult.IsFailure)
			{
				await resultHandler.HandleFailureAsync(nameof(base64Decoder.Decode), decodedResult.Error, job, context.CancellationToken);
				return;
			}
			var decodedContent = decodedResult.Value;

			// Get correct parser for supported type using factory pattern
			var processorResult = processorFactory.GetParser(job.ContentType);
			if (processorResult.IsFailure)
			{
				await resultHandler.HandleFailureAsync(nameof(processorFactory.GetParser), processorResult.Error, job, context.CancellationToken);
				return ;
			}
			var parser = processorResult.Value;

			// Process decoded content 
			var parsedResult = parser.Parse(decodedContent);

			if (parsedResult.IsFailure)
			{
				await resultHandler.HandleFailureAsync(nameof(parser.Parse), parsedResult.Error, job, context.CancellationToken);
				return ;
			}

			await resultHandler.HandleSuccessAsync(new(parsedResult.Value.Count, parsedResult.Value), job, context.CancellationToken);

		}
	}
}
