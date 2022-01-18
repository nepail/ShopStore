using Microsoft.AspNetCore.Authorization;
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
    //[Authorize]
    [Authorize(AuthenticationSchemes = "manager, Cookies")]
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

                if (!CONUSERLIST.LIST.Any(x => x.UserName == ClientName))
                {
                    //建立新的
                    var user = new ConUserModel()
                    {
                        UserAccount = ClientAccount,
                        UserName = ClientName,
                        ConnectionID = ClientID,
                        OnlineTime = DateTime.Now
                    };

                    CONUSERLIST.AddList(user);
                }
                else
                {
                    //更新List裡的ID
                    CONUSERLIST.LIST.Where(x => x.UserName == ClientName).ToList().ForEach(x => x.ConnectionID = ClientID);
                }
            }
        }

        public async override Task OnConnectedAsync()
        {
            //將前台的User加入到清單中
            if (Context.User.Identity.AuthenticationType != "manager")
            {
                AddConUserList();
            }
        }

        public async override Task OnDisconnectedAsync(Exception except)
        {
            //斷線後從List移除
            CONUSERLIST.RemoveList(Context.ConnectionId);
        }



        /// <summary>
        /// 後端向前端通知狀態變更
        /// </summary>
        /// <param name="user"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task SendMessageToFrontedUser(string userAccount, string msg)
        {
            var target = CONUSERLIST.LIST.FirstOrDefault(x => x.UserAccount == userAccount);
            if (target != null)
            {
                await Clients.Clients(target.ConnectionID).SendAsync("SendMessageToFrontedUser", userAccount, msg);
                return;
            }

            //Redis儲存消息，待用戶上線後獲取            

            //在本機上暫存用戶通知//todo
            StoredUserAlert(msg, userAccount);
        }

        /// <summary>
        /// 暫存用戶通知
        /// </summary>
        /// <param name="contentText"></param>
        /// <param name="userAccount"></param>
        private void StoredUserAlert(string contentText, string userAccount)
        {
            string uploadFolder = Path.Combine(WEBHOSTENVIRONMENT.WebRootPath, "userAlert");
            string filePath = Path.Combine(uploadFolder, userAccount + ".txt");
            string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            using StreamWriter sw = File.AppendText(filePath);
            sw.WriteLine($"{now} : {contentText}");
        }

        /// <summary>
        /// 讀取文字檔
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
