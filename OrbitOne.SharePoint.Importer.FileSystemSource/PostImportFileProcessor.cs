using System;
using System.IO;
using log4net;
using OrbitOne.SharePoint.Importer.Domain;

namespace OrbitOne.SharePoint.Importer.FileSystem
{
    public class PostImportFileProcessor
    {
        private readonly ImportSettings m_settings;
        private static ILog log = LogManager.GetLogger(typeof(PostImportFileProcessor));

        public PostImportFileProcessor(ImportSettings settings)
        {
            m_settings = settings;
        }

        public void MoveItem(object sender, ItemProcessedEventArgs e)
        {
            var target = e.Item.OriginalFullName.Replace(m_settings.SourceFolder, m_settings.ArchiveFolder);
            if (m_settings.MoveFiles)
            {
                if (e.Item.IsFile)
                {
                    if (m_settings.Mode == ImportMode.Execute)
                    {
                        EnsureFolder(e.Item.Parent);
                        string shortcutName = Path.Combine(e.Item.Parent.OriginalFullName, "Moved to SharePoint.url");
                        var shortcut = new InternetShortcutFile(e.Location, shortcutName);

                        File.Move(e.Item.OriginalFullName, target);
                        shortcut.Create();
                    }
                }
            }
        }

        void EnsureFolder(ImportFolder folder)
        {
            if (folder == null) return;
            var target = folder.OriginalFullName.Replace(m_settings.SourceFolder, m_settings.ArchiveFolder);
            if (!Directory.Exists(target))
            {
                EnsureFolder(folder.Parent);
                Directory.CreateDirectory(target);
                log.Info("created directory " + target);
            }
        }
    }
}