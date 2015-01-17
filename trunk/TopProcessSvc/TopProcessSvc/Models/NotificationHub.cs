using Microsoft.AspNet.SignalR;

namespace TopProcessSvc.Models
{
    /// <summary>
    /// SignalR push-notifications hub.
    /// </summary>
    public class NotificationHub : Hub
    {
        // Deliberately empty. We need a Hub-derived class for SignalR to work.
    }
}