using System;
using System.Collections.Generic;
using OrbitOne.SharePoint.Importer.Domain;

namespace OrbitOne.SharePoint.Importer.Domain
{
    public class NullUserInformationManager : IMetaDataProvider
    {
        public User GetAuthor(string filename)
        {
            return null;
        }

        public User GetEditor(string filename)
        {
            return null;
        }

        public IDictionary<string, string> GetMetaData(string filename)
        {
            return new Dictionary<string, string>();
        }
    }
}