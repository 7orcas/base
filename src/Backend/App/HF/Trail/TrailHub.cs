using Microsoft.AspNetCore.SignalR;

namespace Backend.App.HF.Trail
{
    public class TrailHub : Hub
    {
        public async Task Subscribe(string trailId)
        {
            await Groups.AddToGroupAsync(
                Context.ConnectionId,
                trailId);
        }
    }
}
