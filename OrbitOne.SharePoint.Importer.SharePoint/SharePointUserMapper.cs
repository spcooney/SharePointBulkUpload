using System.Collections.Generic;
using System.Linq;
using OrbitOne.SharePoint.Importer.Domain;

namespace OrbitOne.SharePoint.Importer.SharePoint
{
    public class SharePointUserMapper : IUserMapper
    {
        private SharePointUserRepository m_repository;
        private IDictionary<string, User> m_mappings;

        public SharePointUserMapper(string siteCollectionUrl)
        {
            m_repository = new SharePointUserRepository(siteCollectionUrl);
            m_mappings = m_repository.GetUsers().ToDictionary(
                user => user.Name.ToUpperInvariant(),
                user => user
                );
        }

        public User Map(string username)
        {
            return m_mappings[username.ToUpperInvariant()];
        }
    }
}
