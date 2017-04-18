using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OrbitOne.SharePoint.Importer.Domain;

namespace OrbitOne.SharePoint.Importer.SharePoint
{
    public class ImportDestinationFactory 
    {
        private readonly ImportSettings m_settings;

        public ImportDestinationFactory(ImportSettings settings)
        {
            m_settings = settings;
        }

        public IImportDestination Create()
        {
            if(!m_settings.CreateFolders)
            {
                return new FlatListSharePointDestination(m_settings);
            }
            return new DefaultSharePointDestination(m_settings);
        }
    }
}
