using System;
using System.Collections.Generic;
using OrbitOne.SharePoint.Importer.Domain;

namespace OrbitOne.SharePoint.Importer.SharePoint
{
    public abstract class SharePointImportDestination : IImportDestination
    {
        protected IDocumentLibraryRepository DocumentLibraryRepository;
        private IList<NameSourcePair>  m_ExistingFilenames;

        public SharePointImportDestination(ImportSettings settings)
        {
            DocumentLibraryRepository = new DocumentLibraryRepository(settings);
        }

        protected IList<NameSourcePair> ExistingFilenames{ get
        {
            if (m_ExistingFilenames == null)
            {
                m_ExistingFilenames = DocumentLibraryRepository.GetFilenamesWithSource();
            }
            return m_ExistingFilenames;
        }}

        public abstract void Import(ImportItem importItem);
        
        public IImportValidator GetValidator()
        {
            return DocumentLibraryRepository.CreateValidator();
        }

        public event EventHandler<ItemProcessedEventArgs> ItemProcessed;

        protected void RaiseItemProcessed(ImportItem item, string location)
        {
            if (ItemProcessed != null)
            {
                var args = new ItemProcessedEventArgs(item, location);
                ItemProcessed(this, args);
            }
        }
    }
}