using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShopStore.Hubs.Models;
using ShopStore.Hubs.Models.Services;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopStore.Hubs
{
    [Authorize(AuthenticationSchemes = "manager")]
    public class ChatHub : Hub
    {
        public string ClientID { get { return Context.ConnectionId; } }

        public string ClientName { get { return Context.User.FindFirstValue(ClaimTypes.Name); } }

        private string Now { get { return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"); } }

        private ConUserService CONUSERLIST;

        public ChatHub(ConUserService conUserService)
        {
            CONUSERLIST = conUserService;
        }

        /// <summary>
        /// User 連線時加入到清單內，之後呼叫 Group 方法傳送在線清單
        /// </summary>
        public async void AddConUserList()
        {
            var _user = new ConUserModel()
            {
                UserName = ClientName,
                ConnectionID = ClientID,
                OnlineTime = DateTime.Now
            };
            
            await Groups.AddToGroupAsync(ClientID, "TestGroup");
            await Clients.Group("TestGroup").SendAsync("GetConList", CONUSERLIST.AddList(_user));
            //await Clients.OthersInGroup("TestGroup").SendAsync("GetConList", selfList);
        }

        public async override Task OnConnectedAsync()
        {            
            AddConUserList();
            //var userName = Context.User.FindFirstValue(ClaimTypes.Name);
            await SendMessageToUser($"{ClientName} 已經連線 ID:", ClientID);            
        }

        public async override Task OnDisconnectedAsync(Exception except)
        {
            await base.OnDisconnectedAsync(except);
            //await SendMessageToUser($"{ClientName} 已經斷線 ID:", ClientID);
            await Clients.Group("TestGroup").SendAsync("GetConList", CONUSERLIST.RemoveList(Context.ConnectionId));
        }


        public async Task SendPrivateMessage(string userId, string message)
        {
            //await Clients.User(userId).SendAsync("ReceivePrivateFromUser", message);
            await Clients.Client(userId).SendAsync("ReceivePrivateFromUser", ClientID, message);
        }

        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }

        public async Task SendMessageToUser(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessageFromUser", user, message);
        }





    }
}
