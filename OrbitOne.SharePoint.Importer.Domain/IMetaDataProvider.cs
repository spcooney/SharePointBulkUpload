using System.Collections.Generic;

namespace OrbitOne.SharePoint.Importer.Domain
{
    public interface IMetaDataProvider
    {
        User GetAuthor(string filename);
        User GetEditor(string filename);
        IDictionary<string, string> GetMetaData(string filename);
    }
}