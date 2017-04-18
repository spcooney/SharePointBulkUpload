using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client;
using User = OrbitOne.SharePoint.Importer.Domain.User;

namespace OrbitOne.SharePoint.Importer.SharePoint
{
    public class SharePointUserRepository
    {
        private string m_url;

        public SharePointUserRepository(string url)
        {
            m_url = url;
        }

        public IList<User> GetUsers()
        {
            using(var context = new ClientContext(m_url))
            {
                var userList = context.Web.SiteUserInfoList;
                var users = userList.GetItems(new CamlQuery { ViewXml = "<View/>" });
                context.Load(users);
                context.ExecuteQuery();

                return users.Select(user => new User
                                                {
                                                    Id = Convert.ToInt32(user["ID"]),
                                                    Name = user["Name"].ToString()
                                                }).ToList();
            }
        }
    }
}
