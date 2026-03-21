using Microsoft.AspNetCore.SignalR;

namespace EXE_BE.API.Hubs
{
    public class RealtimeHub : Hub
    {
        public Task SubscribeUser(Guid userId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"user:{userId}");
        }

        public Task UnsubscribeUser(Guid userId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user:{userId}");
        }

        public Task SubscribeStudyRoom(Guid roomId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"study-room:{roomId}");
        }

        public Task UnsubscribeStudyRoom(Guid roomId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, $"study-room:{roomId}");
        }

        public Task SubscribeVirtualRoom(Guid roomId)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, $"virtual-room:{roomId}");
        }

        public Task UnsubscribeVirtualRoom(Guid roomId)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, $"virtual-room:{roomId}");
        }
    }
}
