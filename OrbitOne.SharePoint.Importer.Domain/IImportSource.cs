using System;

namespace OrbitOne.SharePoint.Importer.Domain
{
    public interface IImportSource
    {
        ImportItem LoadItems();
    }
}