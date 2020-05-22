using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ApplicationCore.Helpers;
using ApplicationCore.Services;
using Microsoft.AspNetCore.SignalR;
using Web.Hubs;

namespace Web.Services
{
    public interface IHubsService
    {
        void SendNotificationToAll(string message);
        IEnumerable<string> GetOnlineUsers();
        Task<Task> SendNotificationParallel(string user);

    }

    public class HubsService
    {
        private readonly IHubClients<NotificationsHub> _hubContext;
        private readonly IConnectionService _connectionService;

        public HubsService(IHubClients<NotificationsHub> hubContext, IConnectionService connectionService)
        {
            _hubContext = hubContext;
            _connectionService = connectionService;
        }

        public void SendNotificationToAll(string message)
        {

        }

        public IEnumerable<string> GetOnlineUsers()
        {

        }

        public async Task<Task> SendNotificationParallel(string user)
        {
            var connections = _connectionService.GetConnections(user);
            try
            {
                if (connections.HasItems())
                {
                    foreach (var conn in connections)
                    {
                        try
                        {

                            await _hubContext.Clients.Client(connectionId).SendAsync(“sendToUser”, model.articleHeading, model.articleContent);
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
