using System;
using System.Diagnostics;
using System.IO;

namespace OrbitOne.SharePoint.Importer.Domain
{
    [DebuggerDisplay("{OriginalFullName}")]
    public class ImportFile : ImportItem
    {
       public ImportFile(){}
        
        public ImportFile(ImportFile original)
        {
            this.SourceFile = original.SourceFile;
            this.Name = original.Name;
            this.CreatedBy = original.CreatedBy;
            this.ModifiedBy = original.ModifiedBy;
            this.MetaData = original.MetaData;
            this.OriginalParent = original.OriginalParent;
            m_fileSize = original.Size;
        }

        private FileInfo m_sourceFile;
        public FileInfo SourceFile
        {
            get { return m_sourceFile; }
            set
            {
                m_sourceFile = value;
                if (value != null)
                {
                    m_fileSize = value.Length;
                    m_originalName = value.Name;
                    m_originalFullName = value.FullName;
                }
            }
        }

        private long m_fileSize;
        private string m_originalName;
        private string m_originalFullName;

        public override long Size
        {
            get { return m_fileSize; }
            set { m_fileSize = value; }
        }

        public override DateTime Created
        {
            get { return SourceFile.CreationTime; }
        }

        public override DateTime Modified
        {
            get { return SourceFile.LastWriteTime; }
        }

        public string Extension
        {
            get
            {
                return Path.GetExtension(Name);
            }
        }

        public override string OriginalName
        {
            get { return m_originalName; }
        }

        public override string OriginalFullName
        {
            get { return m_originalFullName; }
        }

        public override bool IsFile
        {
            get { return true; }
        }

        public Stream OpenRead()
        {
            return SourceFile.OpenRead();
        }
    }
}