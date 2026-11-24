using Microsoft.AspNetCore.SignalR;

namespace Backend.App.HF.Trial
{
    public class TrialHub : Hub
    {
        public async Task Subscribe(string trialId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                trialId);
        }
    }
}
