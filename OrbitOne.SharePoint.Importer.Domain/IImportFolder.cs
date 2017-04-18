using System.Collections.Generic;

namespace OrbitOne.SharePoint.Importer.Domain
{
    public interface IImportFolder
    {
        IList<ImportFile> Files { get; }
        IList<ImportFolder> Folders { get; }
        void Add(ImportItem item);
    }
}