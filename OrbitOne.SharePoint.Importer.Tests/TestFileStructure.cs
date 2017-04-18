using NUnit.Framework;
using OrbitOne.SharePoint.Importer.Domain;

namespace OrbitOne.SharePoint.Importer.Tests
{
    [TestFixture]
    public class TestFileStructure
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [Test]
        public void TestAddFilesToFolder()
        {
            var file = new ImportFile();
            var folder = new ImportFolder();
            folder.Add(file);
            Assert.AreEqual(folder, file.Parent);
        }

        [Test]
        public void TestRelativePath()
        {
            var file = new ImportFile{Name="file"};
            var folder = new ImportFolder{Name="folder"};
            var rootfolder = new ImportFolder{Name="root"};
            folder.Add(file);
            rootfolder.Add(folder);
            Assert.AreEqual("/folder/file", file.ServerRelativePath);
        }
    }
}