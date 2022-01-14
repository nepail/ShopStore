﻿using System.Collections.Generic;

namespace ShopStore.Hubs.Models.Services
{
    public class ConUserService
    {
        public List<ConUserModel> LIST;
        public ConUserService()
        {
            LIST = new List<ConUserModel>();
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
    }
}