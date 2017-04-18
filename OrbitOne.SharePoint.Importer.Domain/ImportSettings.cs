using System;
using System.Net;
namespace OrbitOne.SharePoint.Importer.Domain
{
    public class ImportSettings
    {
        public bool LoggingEnabled{ get; set;}
        public string SourceFolder { get; set; }
        public string DocumentLibrary { get; set; }
        public string SiteUrl { get; set; }

        public bool CreateEmptyFolders{ get; set;}
        public string ArchiveFolder{ get; set; }
        public ImportMode Mode { get; set; }
        public bool ImportHiddenFiles { get; set; }
        public bool CreateFolders { get; set; }
        public bool MoveFiles
        {
            get{ return !String.IsNullOrEmpty(ArchiveFolder);}
        }

        public ICredentials Credentials { get
        {
            return Username == null ? CredentialCache.DefaultCredentials : new NetworkCredential(Username, Password, Domain);
        }
        }
        public AuthenticationMode AuthenticationMode { get; set; }
        public string Password{get; set;}
        public string Username { get; set; }
        public string Domain { get; set; }
    }
}