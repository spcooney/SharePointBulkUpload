using OrbitOne.SharePoint.Importer.Domain;

namespace OrbitOne.SharePoint.Importer.FileSystem
{
    public class FileSystemSourceFactory
    {
        private readonly ImportSettings m_settings;

        public FileSystemSourceFactory(ImportSettings settings)
        {
            m_settings = settings;
        }

        public IImportSource CreateWith(IImportValidator validator)
        {
            return new FileSystemSource(m_settings, validator);
        }
    }
}