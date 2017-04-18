using System.Linq;
using log4net;
using OrbitOne.SharePoint.Importer.Domain;

namespace OrbitOne.SharePoint.Importer.SharePoint
{
    public class DefaultSharePointDestination : SharePointImportDestination
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(DefaultSharePointDestination));

        public DefaultSharePointDestination(ImportSettings settings)
            : base(settings)
        {}

        public override void Import(ImportItem importItem)
        {
            ImportContents(importItem as ImportFolder);
        }

        public void ImportContents(ImportFolder folder)
        {
            if (!folder.IsRoot)
            {
                ImportFolder(folder);
            }
            foreach (var subfolder in folder.Folders)
            {
                ImportContents(subfolder);
            }
            foreach (var file in folder.Files)
            {
                ImportFile(file);
            }
        }

        public void ImportFolder(ImportFolder folder)
        {
            DocumentLibraryRepository.CreateFolder(folder);
        }

        public void ImportFile(ImportFile importFile)
        {
            if (FileExists(importFile))
            {
                Log.Warn("File already exists: " + importFile.OriginalFullName);
            }
            else
            {
                Log.Info("START Processing " + importFile.OriginalFullName);
                var result = DocumentLibraryRepository.CreateFile(importFile);
                ExistingFilenames.Add(new NameSourcePair { Name = importFile.Name, Source = importFile.OriginalFullName });
                if (result.Succeeded)
                {
                    RaiseItemProcessed(importFile, result.Location);
                }
            }
        }

        private bool FileExists(ImportFile importFile)
        {
            return ExistingFilenames.Any(file => file.Source == importFile.OriginalFullName);
        }
    }
}