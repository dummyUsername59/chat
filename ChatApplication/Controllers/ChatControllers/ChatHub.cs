using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;

namespace ChatApplication.Controllers
{
    [HubName("ChatHub")]
    public class ChatHub : Hub
    {
        private static Dictionary<string,string> users = new Dictionary<string,string>();
        private static Dictionary<string, HashSet<string>> userRooms = new Dictionary<string, HashSet<string>>();
        string[] randomNames = new string[] { "alligator", "anteater", "armadillo", "auroch",
            "axolotl","badger", "bat", "beaver", "buffalo", "camel", "chameleon",
            "cheetah", "chipmunk", "chinchilla", "chupacabra", "cormorant",
            "coyote", "crow", "dingo", "dinosaur", "dolphin", "duck",
            "elephant", "ferret", "fox", "frog", "giraffe",
            "gopher", "grizzly", "hedgehog", "hippo", "hyena", "jackal", "ibex",
            "ifrit", "iguana", "koala", "kraken", "lemur", "leopard", "liger", "llama",
            "manatee", "mink", "monkey", "narwhal", "nyan cat", "orangutan",
            "otter", "panda", "penguin",
            "platypus", "python", "pumpkin", "quagga", "rabbit", "raccoon", "rhino",
            "sheep", "shrew", "skunk", "slow loris", "squirrel", "turtle", "walrus", "wolf",
            "wolverine", "wombat"};
         public override Task OnConnected()
        {
            string roomId = Context.QueryString["roomId"];
            if (Context.User!=null)
            {
                users.Add(Context.ConnectionId, Context.User.Identity.Name);
            }
            else
            {
                Random rd = new Random(DateTime.Now.Millisecond);
                users.Add(Context.ConnectionId, "Anonymous "+randomNames[rd.Next(0,randomNames.Count()-1)]);
            }

            if (userRooms.ContainsKey(Context.ConnectionId))
            {
                userRooms[Context.ConnectionId].Add(roomId);
            }
            else
            {
                userRooms.Add(Context.ConnectionId, new HashSet<string>(new string[] { roomId }));
            }

          



           var connectionIds = userRooms.Where(x => x.Value.Contains(roomId)).Select(x => x.Key);
            foreach (var user in userRooms)
            {
                if (user.Value.Contains(roomId))
                {
                    
                    Clients.Client(user.Key).notifyNewEntry(users.Where(x=>connectionIds.Contains(x.Key)).Select(x=>x.Value));
                }
            }

            return base.OnConnected();
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            users.Remove(Context.ConnectionId);
            string roomId = Context.QueryString["roomId"];
            userRooms[Context.ConnectionId].Remove(roomId);


            var connectionIds = userRooms.Where(x => x.Value.Contains(roomId)).Select(x => x.Key);
            foreach (var user in userRooms)
            {
                if (user.Value.Contains(roomId))
                {

                    Clients.Client(user.Key).notifyNewEntry(users.Where(x => connectionIds.Contains(x.Key)).Select(x => x.Value));
                }
            }
            

            return base.OnDisconnected(stopCalled);
        }
        public void Send(string message)
        {
            if(users.ContainsKey(Context.ConnectionId))
            {
                string roomId = Context.QueryString["roomId"];


                foreach(var user in userRooms)
                {
                    if (user.Value.Contains(roomId))
                    {
                        Clients.Client(user.Key).notifyMessage(users[Context.ConnectionId], message);
                    }
                }

            }
            
        }
        
    }
}
