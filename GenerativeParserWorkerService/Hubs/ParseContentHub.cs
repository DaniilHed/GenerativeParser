using Microsoft.AspNetCore.SignalR;


namespace WorkerService.Hubs    
{
    public class ParseContentHub(ILogger logger) : Hub
    {
        public async Task JoinJobGroup(string jobId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, jobId);
            logger.LogInformation("Client with connection id {ConnectionId} joined job group with id {JobId}",Context.ConnectionId,jobId);
        }

        public async Task LeaveJobGroup(string jobId) 
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, jobId);
            logger.LogInformation("Client with connection id {ConnectionId} left job group with id {JobId}", Context.ConnectionId, jobId);
        }

    }
}
