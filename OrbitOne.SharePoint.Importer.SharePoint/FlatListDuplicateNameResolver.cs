using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OrbitOne.SharePoint.Importer.Domain;

namespace OrbitOne.SharePoint.Importer.SharePoint
{
    public class FlatListDuplicateNameResolver
    {
        private readonly IList<string> m_filenames;

        public FlatListDuplicateNameResolver(): this(Enumerable.Empty<string>())
        {}

        public FlatListDuplicateNameResolver(IEnumerable<string> existingFiles)
        {
            m_filenames = existingFiles.ToList();
        }

        public String ResolveName(ImportFile file)
        {
            int i = 0;
            var duplicates = m_filenames.Where(name => name.StartsWith(Path.GetFileNameWithoutExtension(file.Name),StringComparison.InvariantCultureIgnoreCase));
            string uniqueName = file.Name;
            string filenameWithoutExtension = Path.GetFileNameWithoutExtension(file.Name);
            string extension = Path.GetExtension(file.Name);
            while (duplicates.Any(s => s.Equals(uniqueName, StringComparison.InvariantCultureIgnoreCase)))
            {
                i++;
                uniqueName = String.Format("{0}_{1}{2}", filenameWithoutExtension, i, extension);
            }
            m_filenames.Add(uniqueName);
            return uniqueName;
        }
    }
}