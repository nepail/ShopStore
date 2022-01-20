﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using ShopStore.Hubs.Models;
using ShopStore.Hubs.Models.Services;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using static ShopStore.Hubs.Models.Services.ConUserService;

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
            //connectedGroup = new Dictionary<string, GroupUser>();
        }

        /// <summary>
        /// User 連線時加入到清單內，之後呼叫 Group 方法傳送在線列表
        /// </summary>
        private async void AddConUserList(ClaimsIdentity User)
        {
            //var ClientName = Context.User.FindFirstValue(ClaimTypes.Name) ?? String.Empty;

            //從現存的線上列表尋找已連線的使用者
            //var user = CONUSERLIST.LIST.FindIndex(x => x.UserName == ClientName);
            //var userName = User.Name;

            var user = CONUSERLIST.LIST.FindIndex(x => x.UserName == User.Name);

            if (user <= 0)
            {
                //使用者不存在，建立一筆新的
                ConUserModel _user = new ConUserModel()
                {
                    UserName = User.Name,
                    ConnectionID = ClientID,
                    OnlineTime = DateTime.Now
                };

                //這段邏輯有點問題，待查
                await Groups.AddToGroupAsync(ClientID, "ConList");
                //向線上廣播更新後的在線的列表

                await Clients.Group("ConList").SendAsync("GetConList", CONUSERLIST.AddList(_user));

                //將重新連線的 User ConnID 加回存在的Group
                if (CONUSERLIST.connectedGroup.ContainsKey(User.Name))
                {
                    foreach (var a in CONUSERLIST.connectedGroup[User.Name].Group)
                    {
                        await Groups.AddToGroupAsync(ClientID, a.RoomID);
                    }
                }
            }
            else
            {
                //若是重複連線的 User ，仍然再次廣播目前的線上清單
                await Clients.Group("ConList").SendAsync("GetConList", CONUSERLIST.LIST);

                //尋找已連接的Group，將新連線且是重複的 User 加入已存在的 Group
                if (CONUSERLIST.connectedGroup[User.Name].Group.Count > 0)
                {
                    foreach (var a in CONUSERLIST.connectedGroup[User.Name].Group)
                    {
                        await Groups.AddToGroupAsync(ClientID, a.RoomID);
                    }
                }
                //await Groups.AddToGroupAsync(ClientID, )

            }
            //將新建的 User 加入單一使用者群組
            await Groups.AddToGroupAsync(ClientID, User.Name);
        }

        /// <summary>
        /// SingalR 交握完成後進入第一次連線
        /// </summary>
        /// <returns></returns>        
        public async override Task OnConnectedAsync()
        {
            //將後台用戶加入連線清單

            var target = Context.User.Identities.FirstOrDefault(x => x.AuthenticationType == "manager");

            AddConUserList(target);

            //測試用
            await SendMessageToUser($"{ClientName} 已經連線 ID:", ClientID);
        }

        /// <summary>
        /// 斷線處理，若用戶端呼叫 connection.stop() ，except 的值將是 null
        /// </summary>
        /// <param name="except"></param>
        /// <returns></returns>
        public async override Task OnDisconnectedAsync(Exception except)
        {
            await base.OnDisconnectedAsync(except);
            //await SendMessageToUser($"{ClientName} 已經斷線 ID:", ClientID);
            CONUSERLIST.RemoveList(Context.ConnectionId);
            //從線上列表移除斷線的USER & 廣播新的在線列表
            await Clients.Group("ConList").SendAsync("GetConList", CONUSERLIST.RemoveList(Context.ConnectionId));
        }

        /// <summary>
        /// 使用者發起對話
        /// </summary>
        /// <param name="userNameFrom">發起對話者</param>
        /// <param name="userNameTo">對象</param>
        /// <param name="message">訊息</param>
        /// <returns></returns>
        public async Task SendPrivateMessage(string userNameFrom, string userNameTo, string message)
        {
            //await Clients.User(userId).SendAsync("ReceivePrivateFromUser", message);
            //await Clients.Client(userId).SendAsync("ReceivePrivateFromUser", ClientID, message);
            //將要對話的對象加入Group
            var connId = CONUSERLIST.LIST.Find(x => x.UserName == userNameTo).ConnectionID;
            //var connId2 = CONUSERLIST.LIST.Find(x => x.UserName == userNameFrom).ConnectionID;
            await Groups.AddToGroupAsync(connId, userNameFrom);
            //await Groups.AddToGroupAsync(connId2, userNameTo);
            await Clients.Group(userNameTo).SendAsync("ReceivePrivateFromUser", userNameFrom, userNameTo, message);
        }

        /// <summary>
        /// 建立群組
        /// </summary>
        /// <param name="userNameFrom"></param>
        /// <param name="userFromTo"></param>        
        /// <returns></returns>
        public async Task<string> CreateGroup(string userNameFrom, string userFromTo, string groupName)
        {
            //確認發起者是否存在Group
            if (CONUSERLIST.connectedGroup.Any(t => t.Key == userNameFrom))
            {
                //存在Group,回傳Room ID
                return CONUSERLIST.connectedGroup[userNameFrom].Group.FirstOrDefault().RoomID;
            }

            await Groups.AddToGroupAsync(userNameFrom, groupName);
            await Groups.AddToGroupAsync(userFromTo, groupName);

            GroupUser connUser = new GroupUser();
            connUser.Group.Add(new GroupUser.ConnUser { RoomID = groupName });
            CONUSERLIST.connectedGroup.Add(userNameFrom, connUser);
            CONUSERLIST.connectedGroup.Add(userFromTo, connUser);

            return groupName;
        }        

        /// <summary>
        /// 移除群組
        /// </summary>
        /// <param name="groupName"></param>
        /// <returns></returns>
        public string RemoveGroup(string groupName)
        {
            //CONUSERLIST.connectedGroup.Select(o => o.Value.Group.FirstOrDefault().RoomID == groupName);
            //CONUSERLIST.connectedGroup.Remove(x => x.RoomID == groupName);

            //var target = CONUSERLIST.connectedGroup.FirstOrDefault(x => x.Value.Group.FirstOrDefault().RoomID == groupName);
            //if (target.Key != null)
            //{
            //    CONUSERLIST.connectedGroup.Remove(target.Key);

            //}

            //await Groups.RemoveFromGroupAsync()

            return "1";
        }

        /// <summary>
        /// 發送群組對話
        /// </summary>
        /// <param name="groupName"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task SendToGroup(string groupName, string userName, string msg)
        {
            //await Clients.Groups(groupName).SendAsync("GetGroupMsg",userName, msg);
            await Clients.OthersInGroup(groupName).SendAsync("GetGroupMsg", groupName, userName, msg);
        }

        /// <summary>
        /// 庫存量警告
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        //[Authorize]       
        [AllowAnonymous]
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
