using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace OrbitOne.SharePoint.Importer.Domain
{
    [DebuggerDisplayAttribute("{OriginalFullName}")]
    public class ImportFolder : ImportItem, IImportFolder
    {
        public ImportFolder()
        {
            m_items = new List<ImportItem>();
        }

        public DirectoryInfo SourceDirectory { get; set; }

        private IList<ImportItem> m_items;

        public ImportFolder(ImportFolder importFolder) : this()
        {
            this.SourceDirectory = importFolder.SourceDirectory;
            this.OriginalParent = importFolder.OriginalParent;
        }

        public bool IsRoot
        {
            get{ return Parent == null;}
        }

        public IList<ImportFile> Files
        {
            get { return m_items.OfType<ImportFile>().ToList(); }
        }

        public IList<ImportFolder> Folders
        {
            get { return m_items.OfType<ImportFolder>().ToList(); }
        }

        public override string OriginalName
        {
            get { return SourceDirectory.Name; }
        }

        public override string OriginalFullName
        {
            get { return SourceDirectory.FullName; }
        }

        public override bool IsFile
        {
            get { return false; }
        }

        public override long Size
        {
            get { return m_items.Sum(item => item.Size); }
            set{ }
        }

        public override DateTime Created
        {
            get { return SourceDirectory.CreationTime; }
        }

        public override DateTime Modified
        {
            get { return SourceDirectory.LastWriteTime; }
        }

        public void Add(ImportItem item)
        {
            item.Parent = this;
            if(item.OriginalParent == null)
            {
                item.OriginalParent = this;
            }
            m_items.Add(item);
        }

        public int FileCount
        {
            get
            {
                return Folders.Sum(f => f.FileCount) + Files.Count();
            }
        }
    }
}