#region 功能與歷史修改描述

/*
    描述:前台訊息通知
    建立日期:2022-01-18

    描述:程式碼風格調整
    修改日期:2022-01-20

 */

#endregion

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using ShopStore.Hubs.Models;
using ShopStore.Hubs.Models.Services;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShopStore.Hubs
{
    public class ServerHub : Hub
    {
        public string ClientID { get { return Context.ConnectionId; } }
        public string ClientName { get { return Context.User.FindFirstValue(ClaimTypes.Name); } }

        public string ClientAccount { get { return Context.User.FindFirstValue("Account"); } }

        private string Now { get { return DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff"); } }

        private ConUserService CONUSERLIST;

        private readonly IDistributedCache REDIS;

        private readonly IWebHostEnvironment WEBHOSTENVIRONMENT;
        public ServerHub(ConUserService conUserService, IDistributedCache redis, IWebHostEnvironment webHostEnvironment)
        {
            CONUSERLIST = conUserService;
            REDIS = redis;
            WEBHOSTENVIRONMENT = webHostEnvironment;
        }

        private async void AddConUserList()
        {
            if (ClientName != null)
            {
                //加入前台User
                if (!CONUSERLIST.ServerList.Any(x => x.UserName == ClientName))
                {
                    var target = Context.User.Identities.FirstOrDefault(x => x.AuthenticationType == "Cookies");

                    if(target != null)
                    {
                        var user = new ConUserModel()
                        {
                            UserAccount = target.Claims.FirstOrDefault(x => x.Type == "Account").Value,
                            UserName = target.Claims.FirstOrDefault(x => x.Type == ClaimTypes.Name).Value,
                            ConnectionID = ClientID,
                            OnlineTime = DateTime.Now
                        };

                        CONUSERLIST.AddToServerList(user);
                    }                    

                }
                else
                {
                    //更新List裡的ID
                    CONUSERLIST.ServerList.Where(x => x.UserName == ClientName).ToList().ForEach(x => x.ConnectionID = ClientID);
                }
            }
        }

        public async override Task OnConnectedAsync()
        {
            //將前台的User加入到清單中            
            AddConUserList();
        }

        public async override Task OnDisconnectedAsync(Exception except)
        {
            //斷線後從List移除
            CONUSERLIST.RemoveFromServerList(Context.ConnectionId);
        }

        /// <summary>
        /// 後端向前端通知狀態變更
        /// </summary>
        /// <param name="user"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task SendMessageToFrontedUser(string userAccount, string orderId, string stateMsg)
        {
            var target = CONUSERLIST.ServerList.FirstOrDefault(x => x.UserAccount == userAccount);
            if (target != null)
            {
                await Clients.Clients(target.ConnectionID).SendAsync("SendMessageToFrontedUser", orderId, stateMsg);
                return;
            }

            //Redis儲存消息，待用戶上線後獲取            

            //在本機上暫存用戶通知//todo
            StoredUserAlert(userAccount, orderId, stateMsg);
        }

        /// <summary>
        /// 暫存用戶通知
        /// </summary>
        /// <param name="contentText"></param>
        /// <param name="userAccount"></param>
        private void StoredUserAlert(string userAccount, string orderId, string stateMsg)
        {
            string uploadFolder = Path.Combine(WEBHOSTENVIRONMENT.WebRootPath, "userAlert");
            string filePath = Path.Combine(uploadFolder, userAccount + ".txt");
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using StreamWriter sw = File.AppendText(filePath);
            sw.WriteLine($"{now},#{orderId},{stateMsg}");
        }

        /// <summary>
        /// 讀取文字檔(暫未使用)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private string ReadProductContent(string id)
        {
            string filePath = @$"{Path.Combine(WEBHOSTENVIRONMENT.WebRootPath, "content\\")}{id}.txt";
            string contentTxt = null;

            using StreamReader reader = new StreamReader(filePath);
            if (System.IO.File.Exists(filePath))
            {
                contentTxt = reader.ReadToEnd();
            }

            return contentTxt;
        }


        /// <summary>
        /// 向後台發送庫存量預警
        /// </summary>
        /// <param name="user"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}
