using System.IO;
using log4net;
using OrbitOne.SharePoint.Importer.Domain;

namespace OrbitOne.SharePoint.Importer.FileSystem
{
    public class FileSystemSource : IImportSource
    {
        private static ILog log = LogManager.GetLogger(typeof(FileSystemSource));
        private readonly ImportSettings m_settings;
        private IImportValidator m_validator;
        private FileNameConverter m_filenameConverter;

        public FileSystemSource(ImportSettings settings, IImportValidator validator)
        {
            log.Info("Initializing data source: File system: " + settings.SourceFolder);
            m_settings = settings;
            m_validator = validator;
            MetaDataProvider = new NullUserInformationManager();
            m_filenameConverter = new FileNameConverter
                                      {
                                          MaximumFileNameLenght = validator.MaximumFileNameLength,
                                          IllegalCharacters = validator.IllegalCharacters
                                      };
        }

        public IMetaDataProvider MetaDataProvider { get; set; }

        public ImportItem LoadItems()
        {
            return Load(m_settings.SourceFolder);
        }

        private ImportItem Load(string name)
        {
            var directory = new DirectoryInfo(name);
            var item = new ImportFolder
                           {
                               SourceDirectory= directory,
                               Name = m_filenameConverter.Convert(directory.Name),
                               CreatedBy = MetaDataProvider.GetAuthor(directory.FullName),
                               ModifiedBy = MetaDataProvider.GetEditor(directory.FullName),
                               MetaData = MetaDataProvider.GetMetaData(directory.FullName)
                           };

            var result = m_validator.Validate(item);
            if (result.IsValid)
            {
                foreach (var folder in directory.GetDirectories())
                {
                    if ((folder.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden || m_settings.ImportHiddenFiles)
                    {
                        item.Add(Load(folder.FullName));
                    }
                }

                foreach (var file in directory.GetFiles())
                {
                    if ((file.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden || m_settings.ImportHiddenFiles)
                    {
                        var importFile = new ImportFile
                                             {
                                                 SourceFile = file,
                                                 Name = m_filenameConverter.Convert(file.Name),
                                                 CreatedBy = MetaDataProvider.GetAuthor(file.FullName),
                                                 ModifiedBy = MetaDataProvider.GetEditor(file.FullName),
                                                 MetaData = MetaDataProvider.GetMetaData(file.FullName),
                                             };
                        var fileResult = m_validator.Validate(importFile);
                        if (fileResult.IsValid)
                        {
                            item.Add(importFile);
                        }
                        else
                        {
                            Log(fileResult);
                        }
                    }
                }
            }
            else
            {
                Log(result);
            }
            return item;
        }

        void Log(ValidationResult result)
        {
            foreach(var error in result.Errors)
            {
                log.Error(error + "  " + result.Source);
            }
            foreach(var warning in result.Warnings)
            {
                log.Warn(warning + "  " + result.Source );
            }
        }
    }
}