using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using log4net;
using Microsoft.SharePoint.Client;
using OrbitOne.SharePoint.Importer.Domain;

namespace OrbitOne.SharePoint.Importer.SharePoint
{
    public interface IDocumentLibraryRepository
    {
        CreateFileResult CreateFile(ImportFile importFile);
        void CreateFolder(ImportFolder folder);
        IImportValidator CreateValidator();
        IList<NameSourcePair> GetFilenamesWithSource();
    }

    public class CreateFileResult
    {
        public string Location { get; set; }
        public bool Succeeded { get; set; }
    }

    public class DocumentLibraryRepository : IDocumentLibraryRepository
    {
        private readonly ImportSettings m_settings;
        private static ILog log = LogManager.GetLogger(typeof(DocumentLibraryRepository));
        private const bool ShouldOverwrite = false;
        private string m_serverRelativeListUrl;
        private IDictionary<string, string> m_availableFields;

        public string ApplicationUrl { get; set; }

        private readonly Func<ClientContext> CreateContext;
        private bool m_Initialized = false;

        public DocumentLibraryRepository(ImportSettings settings)
        {
            m_settings = settings;
            CreateContext = () =>
                                {
                                    var context = new ClientContext(m_settings.SiteUrl);
                                    context.ExecutingWebRequest += (s, e) => e.WebRequestExecutor.RequestHeaders.Add("X-FORMS_BASED_AUTH_ACCEPTED", "f");

                                    if (settings.AuthenticationMode == Domain.AuthenticationMode.Windows)
                                    {
                                        context.Credentials = m_settings.Credentials;
                                     
                                    }
                                    else if(settings.AuthenticationMode == Domain.AuthenticationMode.Forms)
                                    {
                                        context.AuthenticationMode = ClientAuthenticationMode.FormsAuthentication;
                                        context.FormsAuthenticationLoginInfo = new FormsAuthenticationLoginInfo(settings.Username, settings.Password);
                                    }
                                    return context;
                                };

            var uri = new Uri(settings.SiteUrl);
            ApplicationUrl = string.Format("{0}://{1}", uri.Scheme, uri.Host);
        }

        private void EnsureInitialized()
        {
            if(!m_Initialized)
            {
                using (var context = CreateContext())
                {
                    m_availableFields = GetAvailableFields(context);
                    EnsureColumn("_Source", context);
                    var list = context.Web.Lists.GetByTitle(m_settings.DocumentLibrary);
                    context.Load(context.Web, web => web.ServerRelativeUrl);
                    context.Load(list);
                    context.Load(list.RootFolder);
                    context.ExecuteQuery();
                    m_serverRelativeListUrl = list.RootFolder.ServerRelativeUrl;
                }
                m_Initialized = true;
            }
        }

        private void EnsureColumn(string fieldName, ClientContext context)
        {
            if (m_availableFields.ContainsKey(fieldName))
            {
                return;
            }
            var sourceField = context.Site.RootWeb.Fields.GetByInternalNameOrTitle(fieldName);
            context.Load(sourceField);
            var list = context.Web.Lists.GetByTitle(m_settings.DocumentLibrary);
            list.Fields.Add(sourceField);
            context.ExecuteQuery();
        }

        public CreateFileResult CreateFile(ImportFile file)
        {
            EnsureInitialized();
            using (var context = CreateContext())
            {
                var location = ApplicationUrl + m_serverRelativeListUrl + file.Parent.ServerRelativePath;
                bool fileCreated = false;
                bool succeeded = true;
                try
                {
                    CreateFile(file, context);
                    fileCreated = true;
                    ApplyMetaData(file, context);
                }
                catch (Exception e)
                {
                    succeeded = false;
                    log.Error(e);
                    if(fileCreated)
                    {
                        log.Info("removing " + location);
                        DeleteFile(file,context);
                    }
                }
                return new CreateFileResult{Succeeded = succeeded,Location=location};
            }
        }

        private void DeleteFile(ImportFile file, ClientContext context)
        {
            string serverRelativeFileUrl = string.Concat(m_serverRelativeListUrl, file.ServerRelativePath);
            var f = context.Web.GetFileByServerRelativeUrl(serverRelativeFileUrl);
            f.DeleteObject();
            context.ExecuteQuery();
        }

        void ApplyMetaData(ImportFile importFile, ClientContext context)
        {
            string serverRelativeFileUrl = string.Concat(m_serverRelativeListUrl, importFile.ServerRelativePath);
            var file = context.Web.GetFileByServerRelativeUrl(serverRelativeFileUrl);

            context.Load(
                    file,
                    f => f.ListItemAllFields,
                    f => f.ServerRelativeUrl
                );

            var listItem = file.ListItemAllFields;
            MapMembers(importFile, listItem);

            listItem.Update();
            context.ExecuteQuery();
        }

        void CreateFile(ImportFile importFile, ClientContext context)
        {
            string serverRelativeFileUrl = string.Concat(m_serverRelativeListUrl, importFile.ServerRelativePath);

            using (var stream = importFile.OpenRead())
            {
                if (m_settings.Mode == ImportMode.Execute)
                {
                    log.Info("Saving file to SharePoint: " + ApplicationUrl + serverRelativeFileUrl);
                    File.SaveBinaryDirect(context, serverRelativeFileUrl, stream, ShouldOverwrite);
                    log.Info("Succeeded");
                }
            }
        }

        private void MapMembers(ImportFile importFile, ListItem listItem)
        {
            listItem["Created"] = importFile.Created;
            listItem["Modified"] = importFile.Modified;

            if (importFile.ModifiedBy != null)
            {
                listItem["Editor"] = new FieldUserValue { LookupId = importFile.ModifiedBy.Id };
            }
            if (importFile.CreatedBy != null)
            {
                listItem["Author"] = new FieldUserValue { LookupId = importFile.CreatedBy.Id };
            }

            listItem["_Source"] = importFile.OriginalFullName;

            foreach (var foo in importFile.MetaData)
            {
                if (m_availableFields.ContainsKey(foo.Key))
                {
                    listItem[foo.Key] = foo.Value;
                }
                else
                {
                    log.Warn("Could not import " + foo.Key + ". Field not found");
                }
            }
        }

        public long MaximumFileSize
        {
            get { return 200 * 1024 * 1024; }
        }

        public void CreateFolder(ImportFolder folder)
        {
            EnsureInitialized();
            using (var context = CreateContext())
            {
                string parentFolderName = m_serverRelativeListUrl + folder.Parent.ServerRelativePath;
                var rootFolder = context.Web.GetFolderByServerRelativeUrl(parentFolderName);
                context.Load(rootFolder, f => f.Folders, f => f.Name);
                context.ExecuteQuery();
                var parentFolder = rootFolder.Folders.ToList();

                if (!parentFolder.Any(f => f.Name.Equals(folder.Name)))
                {
                    if (m_settings.Mode == ImportMode.Execute)
                    {
                        rootFolder.Folders.Add(m_serverRelativeListUrl + folder.ServerRelativePath);
                    }
                }
                rootFolder.Update();
                context.ExecuteQuery();
            }
        }

        public IImportValidator CreateValidator()
        {
            return new DefaultValidator
                       {
                           BlockedFileExtensions = Constants.BlockedFileExtensions,
                           IllegalCharacters = Constants.IllegalCharacters,
                           MaximumFileSize = this.MaximumFileSize,
                           DocumentLibraryExists = this.DocumentLibraryExists()
                       };
        }

        private bool DocumentLibraryExists()
        {
            using (var context = CreateContext())
            {
                try
                {
                    var list = context.Web.Lists.GetByTitle(m_settings.DocumentLibrary);
                    context.Load(list, l => l.Title);
                    context.ExecuteQuery();
                    return list.Title == m_settings.DocumentLibrary;
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        private IDictionary<string, string> GetAvailableFields(ClientContext context)
        {
            var list = context.Web.Lists.GetByTitle(m_settings.DocumentLibrary);
            context.Load(list, l => l.Fields);
            context.ExecuteQuery();
            return list.Fields.ToDictionary(field => field.InternalName, field => field.TypeAsString);
        }


        public IList<NameSourcePair> GetFilenamesWithSource()
        {
            EnsureInitialized();
            using (var context = CreateContext())
            {
                var result = new List<NameSourcePair>();
                var list = context.Web.Lists.GetByTitle(m_settings.DocumentLibrary);
                ListItemCollectionPosition itemPosition = null;

                while (true)
                {
                    var q = new CamlQuery
                                {
                                    ViewXml = "<View><RowLimit>5000</RowLimit></View>",
                                    ListItemCollectionPosition = itemPosition
                                };

                    var items = list.GetItems(q);
                    context.Load(list);

                    context.Load(items,
                                foo => foo.ListItemCollectionPosition,
                                 i => i.Include(
                                     item => item["FileLeafRef"],
                                     item => item["_Source"]
                                          )
                        );
                    context.ExecuteQuery();
                    itemPosition = items.ListItemCollectionPosition;
                    result.AddRange(items.ToList().Select(i => new NameSourcePair
                                             {
                                                 Name = i["FileLeafRef"].ToString(),
                                                 Source = i["_Source"] == null ? string.Empty : i["_Source"].ToString()
                                             })
                                             .ToList());
                    if (itemPosition == null)
                    {
                        break;
                    }
                }
                return result;
            }
        }
    }
}