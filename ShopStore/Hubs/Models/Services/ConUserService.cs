using System.Collections.Generic;

namespace ShopStore.Hubs.Models.Services
{
    public class ConUserService
    {
        public List<ConUserModel> LIST;
        public List<ConUserModel> ServerList;
        public Dictionary<string, GroupUser> connectedGroup;


        public ConUserService()
        {
            LIST = new List<ConUserModel>();
            ServerList = new List<ConUserModel>();
            connectedGroup = new Dictionary<string, GroupUser>();
        }

        public List<ConUserModel> AddList(ConUserModel user)
        {
            LIST.Add(user);
            return LIST;
        }

        public List<ConUserModel> RemoveList(string connectionId)
        {
            var index = LIST.FindIndex(x => x.ConnectionID == connectionId);
            LIST.RemoveAt(index);
            return LIST;
        }

        public List<ConUserModel> AddToServerList(ConUserModel user)
        {
            ServerList.Add(user);
            return ServerList;
        }

        public List<ConUserModel> RemoveFromServerList(string connectionId)
        {
            var index = ServerList.FindIndex(x => x.ConnectionID == connectionId);
            ServerList.RemoveAt(index);
            return ServerList;
        }

        public class GroupUser
        {
            public GroupUser()
            {
                Group = new List<ConnUser>();
            }

            public List<ConnUser> Group { get; set; }

            public class ConnUser
            {
                public string RoomID { get; set; }

            }
        }
    }
}
