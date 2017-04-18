using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using OrbitOne.SharePoint.Importer.Domain;

namespace OrbitOne.SharePoint.Importer.SharePoint
{
    public class FlatListSharePointDestination : SharePointImportDestination
    {
        protected static readonly ILog Log = LogManager.GetLogger(typeof(FlatListSharePointDestination));
        private FlatListDuplicateNameResolver m_nameResolver;
        private FlatListDuplicateNameResolver NameResolver
        {
            get
            {
                if (m_nameResolver == null)
                {
                    m_nameResolver = new FlatListDuplicateNameResolver(ExistingFilenames.Select(p => p.Name));        
                }
                return m_nameResolver;
            }
        }
        
        
        public FlatListSharePointDestination(ImportSettings settings)
            : base(settings)
        {
            
        }

        public override void Import(ImportItem importItem)
        {
            var folder = new ImportFolder(importItem as ImportFolder);
            var flatList = GetFiles(importItem as ImportFolder);

            foreach (var file in flatList)
            {
                var newFile = new ImportFile(file);
                newFile.Name = NameResolver.ResolveName(file);
                folder.Add(newFile);
                ImportFile(file, newFile);
            }
        }

        private void ImportFile(ImportFile orig, ImportFile file)
        {
            if (FileExists(file))
            {
                Log.Warn("File already exists and is skipped: " + file.OriginalFullName);
                return;
            }
            Log.Info("START Processing " + file.OriginalFullName);
            var result = DocumentLibraryRepository.CreateFile(file);
            ExistingFilenames.Add(new NameSourcePair { Name = file.Name, Source = file.OriginalFullName });
            if(result.Succeeded)
            {
                RaiseItemProcessed(orig, result.Location);
            }

        }

        private bool FileExists(ImportFile file)
        {
            return ExistingFilenames.Any(f => f.Source.Equals(file.OriginalFullName,StringComparison.InvariantCultureIgnoreCase));
        }

        private IEnumerable<ImportFile> GetFiles(ImportFolder folder)
        {

            foreach (var fol in folder.Folders)
            {
                foreach (var file in GetFiles(fol))
                {
                    yield return file;
                }
            }
            foreach (var file in folder.Files)
            {
                yield return file;
            }
        }
    }
}