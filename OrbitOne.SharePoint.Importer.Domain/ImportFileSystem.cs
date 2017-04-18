using System.Collections.Generic;

namespace OrbitOne.SharePoint.Importer.Domain
{
    public class ImportFileSystem
    {
        private IEnumerable<ImportItem> m_items;

        public IEnumerable<ImportItem> Items
        {
            get { return m_items; }
        }
    }
}