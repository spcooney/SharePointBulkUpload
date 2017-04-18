using System;

namespace OrbitOne.SharePoint.Importer.Domain
{
    public class ItemProcessedEventArgs : EventArgs
    {
        public ImportItem Item { get; private set; }
        public string Location { get; set; }

        public ItemProcessedEventArgs(ImportItem item, string location)
        {
            Item = item;
            Location = location;
        }
    }
}