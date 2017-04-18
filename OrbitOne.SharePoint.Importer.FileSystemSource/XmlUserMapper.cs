using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Linq;
using OrbitOne.SharePoint.Importer.Domain;

namespace OrbitOne.SharePoint.Importer.FileSystem
{
    public class XmlUserMapper : IUserMapper
    {
        private IDictionary<string, User> m_mapping;

        public XmlUserMapper(string filename)
        {
            XDocument.Load(filename).Elements("UserMapping").ToDictionary(
                map => map.Element("username").Value.ToUpperInvariant(),
                map => new User
                           {
                               Id = Convert.ToInt32(map.Element("userId").Value),
                               Name = map.Element("username").Value
                           }
                );
        }

        public User Map(string username)
        {
            return m_mapping[username.ToUpperInvariant()];
        }
    }
}