using System;
using log4net;
using OrbitOne.SharePoint.Importer.Domain;
using OrbitOne.SharePoint.Importer.FileSystem;
using OrbitOne.SharePoint.Importer.SharePoint;

namespace OrbitOne.SharePoint.Importer
{
    public class DocumentImporter
    {
        private static ILog log = LogManager.GetLogger(typeof(DocumentImporter));
        private readonly ImportSettings m_settings;

        public DocumentImporter(ImportSettings settings)
        {
            m_settings = settings;
        }

        public void Execute()
        {
            log.Info("Configuring application");

            log.Info("Initializing Sharepoint Connection");
            var sharePointFactory = new ImportDestinationFactory(m_settings);
            var sharepoint = sharePointFactory.Create();

            log.Info("Initializing Validation");
            var validator = sharepoint.GetValidator();
            if (!validator.IsValid)
            {
                log.Error("The document library "+m_settings.DocumentLibrary +" does not exist");
                return;
            }
            var factory = new FileSystemSourceFactory(m_settings);
            var source = factory.CreateWith(validator);

            var processor = new PostImportFileProcessor(m_settings);

            sharepoint.ItemProcessed += (s, args) => log.Info("END Processing " + args.Item.OriginalFullName);

            if (m_settings.MoveFiles)
            {
                sharepoint.ItemProcessed += processor.MoveItem;
            }

            var items = source.LoadItems();

            DisplayImportStatistics(items);

            if (m_settings.Mode == ImportMode.Execute)
            {
                log.Info("Start Import");
                sharepoint.Import(items);
                log.Info("Import finished");
            }
            else if(m_settings.Mode == ImportMode.Analyse)
            {
                
            }
        }

        private static void DisplayImportStatistics(ImportItem items)
        {
            var s = new ImportStatistics(items);

            log.Info("Number of files: " + s.NumberOfFiles);
            log.Info("Number of folders: " + s.NumberOfDirectories);
            log.Info("Number of empty folders: " + s.NumberOfEmptyDirectories);
            log.Info("Total file size: " + GetSizeString(s.TotalFileSize));
        }

        static string GetSizeString(long bytes)
        {
            if (bytes >= 1073741824.0)
            {
                return String.Format("{0:##.##}", bytes / 1073741824.0) + " GB";
            }
            if (bytes >= 1048576.0)
            {
                return String.Format("{0:##.##}", bytes / 1048576.0) + " MB";
            }
            if (bytes >= 1024)
            {
                return String.Format("{0:##.##}", bytes / 1024) + " KB";
            }
            return bytes + " bytes";
        }
    }
}