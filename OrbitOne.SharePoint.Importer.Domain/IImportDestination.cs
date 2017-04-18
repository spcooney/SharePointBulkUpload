using System;

namespace OrbitOne.SharePoint.Importer.Domain
{
    public interface IImportDestination
    {
        void Import(ImportItem importFile);
        IImportValidator GetValidator();
        event EventHandler<ItemProcessedEventArgs> ItemProcessed;
    }
}