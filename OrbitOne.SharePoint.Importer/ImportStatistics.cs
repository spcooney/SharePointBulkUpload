using System.Collections.Generic;
using System.Linq;
using OrbitOne.SharePoint.Importer.Domain;

namespace OrbitOne.SharePoint.Importer
{
    public class ImportStatistics
    {
        public int NumberOfFiles { get; set; }
        public int NumberOfDirectories { get; set; }
        public int NumberOfEmptyDirectories { get; set; }
        public long TotalFileSize { get; set; }
        public IDictionary<string, string> ProblematicFiles { get; set; }

        public ImportStatistics(ImportItem item)
        {
            TotalFileSize = item.Size;
            NumberOfFiles = CountFiles(item as ImportFolder);
            NumberOfDirectories = CountFolders(item as ImportFolder);
            NumberOfEmptyDirectories = CountEmptyDirectories(item as ImportFolder);
        }

        private int CountEmptyDirectories(ImportFolder importFolder)
        {
            return importFolder.Folders.Where(f => f.Files.Count == 0).Count() + importFolder.Folders.Sum(f => CountEmptyDirectories(f));
        }

        private int CountFiles(ImportFolder folder)
        {
            return folder.Files.Count + folder.Folders.Sum(f => CountFiles(f));
        }

        private int CountFolders(ImportFolder folder)
        {
            return folder.Folders.Count +  folder.Folders.Sum(f => CountFolders(f));
        }
    }
}