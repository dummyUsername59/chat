using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Helpers;

namespace ChatApplication.Controllers.ChatControllers
{

    [HubName("RoomHub")]
    public class RoomsHub:Hub
    {
        IDictionary<string, KeyValuePair<string,DateTime>> rooms;
        public RoomsHub()
        {
            rooms = DataProvider.CacheProvider.Instance.GetAll();
        }
        public void Send(string message)
        {

        }
        public override Task OnConnected()
        {
            rooms = DataProvider.CacheProvider.Instance.GetAll();
            Clients.All.updateRooms(rooms.Select(x=> new { Key = x.Key, Value = x.Value.Key}));
            return base.OnConnected();
        }
        public override Task OnReconnected()
        {
            rooms = DataProvider.CacheProvider.Instance.GetAll();
            Clients.All.updateRooms(rooms.Select(x => new { Key = x.Key, Value = x.Value.Key }));
            return base.OnReconnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            return base.OnDisconnected(stopCalled);
        }
        
    }
}