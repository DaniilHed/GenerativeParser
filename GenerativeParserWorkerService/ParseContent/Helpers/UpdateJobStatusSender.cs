using Infrastructure.ParseContent.Enums;
using Microsoft.AspNetCore.SignalR;
using WorkerService.Hubs;

namespace WorkerService.ParseContent.Helpers
{
	public static class UpdateJobStatusSender
	{
		public static async Task SendJobStatusUpdateAsync(this IHubContext<ParseContentHub> hubContext, Guid jobId, JobStatus status)
		{
			await hubContext.Clients.Group(jobId.ToString()).SendAsync("ReceiveJobStatusUpdate", status.ToString());
		}
	}
}
