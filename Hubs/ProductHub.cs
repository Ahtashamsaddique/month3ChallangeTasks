using Microsoft.AspNetCore.SignalR;

namespace WebApplication2.Hubs
{
    public class ProductHub : Hub
    {
        public async Task SendProductUpdate(string message)
        {
            await Clients.All.SendAsync("ReceiveProductUpdate", message);
        }
    }
}
