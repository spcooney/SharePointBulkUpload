using System;
using System.Collections.Generic;

namespace OrbitOne.SharePoint.Importer.Domain
{
    public abstract class ImportItem
    {
        public abstract string OriginalName { get; }
        public abstract string OriginalFullName { get;}

        public ImportFolder Parent { get; set; }
        public ImportFolder OriginalParent { get; set; }
        
        public abstract bool IsFile { get; }
        public abstract long Size { get; set; }
        public User CreatedBy { get; set; }
        public User ModifiedBy { get; set; }
        public abstract DateTime Created { get;}
        public abstract DateTime Modified { get; }
        
        private IDictionary<string, string> m_metaData = new Dictionary<string, string>();
        public IDictionary<string, string> MetaData
        {
            get { return m_metaData; }
            set { m_metaData = value; }
        }

        public string Name { get; set; }

        public string ServerRelativePath
        {
            get
            {
                if (Parent == null) 
                {
                    return "";
                }
                return String.Concat(Parent.ServerRelativePath, "/", Name);
            }
        }
        
        public string RelativeFilePath
        {
            get
            {
                if (OriginalParent == null)
                {
                    return "";
                }
                return String.Concat(OriginalParent.RelativeFilePath, "\\", OriginalName).TrimStart('\\');
            }
        }
    }
}