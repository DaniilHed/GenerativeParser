using Common;
using Common.Errors;
using Contracts.DTOs;
using Infrastructure.ParseContent.DataBaseContext;
using Infrastructure.ParseContent.Entities;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using WorkerService.Hubs;
using static MassTransit.ValidationResultExtensions;


namespace WorkerService.ParseContent.Helpers
{
	public class JobResultHandler(
		ILogger<JobResultHandler> logger,
		ParseContentDbContext dbContext,
		IHubContext<ParseContentHub> hubContext)
	{
		public async Task HandleFailureAsync (string methodName,Error error,ParseContentJob job, CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(job);
			ArgumentException.ThrowIfNullOrWhiteSpace(job.ContentType);

			logger.LogWarning(
					"{MethodName} failed for ContentType: {ContentType}. Code: {ErrorCode}, Description: {ErrorDescription}",
					methodName,
					job.ContentType,
					error.Code,
					error.Description);
			job.SetFailed(error.Code, error.Description!);
			await dbContext.SaveChangesAsync(cancellationToken);
			await hubContext.SendJobStatusUpdateAsync(job.Id, job.Status);
			return;
		}

		public async Task HandleSuccessAsync(ParseContentResult result, ParseContentJob job,CancellationToken cancellationToken)
		{
			ArgumentNullException.ThrowIfNull(result);
			ArgumentNullException.ThrowIfNull(job);
			logger.LogInformation(
				"Successfully parsed {RecordCount} records for ContentType: {ContentType}",
				result.RecordCount,
				job.ContentType);
			job.SetCompleted(result);

			await dbContext.SaveChangesAsync(cancellationToken);
			await hubContext.SendJobStatusUpdateAsync(job.Id, job.Status);
		}
	}
}
