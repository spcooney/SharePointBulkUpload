using System.Linq;
using Moq;
using NUnit.Framework;
using OrbitOne.SharePoint.Importer.Domain;
using OrbitOne.SharePoint.Importer.SharePoint;

namespace OrbitOne.SharePoint.Importer.Tests
{
    [TestFixture]
    public class TestDuplicateNameResolver
    {
        [Test]
        public void TestNonDuplicateName()
        {
            var file = new ImportFile
                           {
                               Name = "Test.docx"
                           };
            
            var nameResolver = new FlatListDuplicateNameResolver();

            Assert.AreEqual(file.Name,nameResolver.ResolveName(file));
        }

        [Test]
        public void TestCreateUniqueName()
        {
            var file = new ImportFile
            {
                Name = "Test.docx"
            };
            var file2 = new ImportFile
            {
                Name = "Test.docx"
            };

            var sharepoint = new Mock<IDocumentLibraryRepository>();
            var nameResolver = new FlatListDuplicateNameResolver();
            string first = nameResolver.ResolveName(file);
            Assert.AreNotEqual(file2.Name, nameResolver.ResolveName(file2));
        }


        [Test]
        public void TestUniqueNameIsCorrect()
        {
            string expected = "Test_1.docx";
            var file = new ImportFile
            {
                Name = "Test.docx"
            };
            var file2 = new ImportFile
            {
                Name = "Test.docx"
            };

            var nameResolver = new FlatListDuplicateNameResolver();
            string first = nameResolver.ResolveName(file);
            Assert.AreEqual(expected, nameResolver.ResolveName(file2));
        }

        [Test]
        public void TestNoExtension()
        {
            var file = new ImportFile
            {
                Name = "Test"
            };
            var file2 = new ImportFile
            {
                Name = "Test"
            };

            var nameResolver = new FlatListDuplicateNameResolver();
            string first = nameResolver.ResolveName(file);
            Assert.AreEqual("Test_1", nameResolver.ResolveName(file2));
        }

        [Test]
        public void TestCreateWithExistingFiles()
        {
            var existingFiles = new[]
                                    {
                                        "Test.docx",  
                                        "Test_1.docx",
                                        "Test_2.docx"
                                    };
            var file = new ImportFile
            {
                Name = "Test.docx",
            };
            
            var nameResolver = new FlatListDuplicateNameResolver(existingFiles);
            string first = nameResolver.ResolveName(file);
            Assert.AreEqual("Test_3.docx", first);
        }

        [Test]
        public void TestCreateMultipleWithExistingFiles()
        {
            var existingFiles = Enumerable.Empty<string>();
            var file = new ImportFile
            {
                Name = "Test.docx",
            };
            var file2 = new ImportFile
            {
                Name = "Test.docx",
            };
            var nameResolver = new FlatListDuplicateNameResolver(existingFiles);
            string first = nameResolver.ResolveName(file);
            string second = nameResolver.ResolveName(file2);
            Assert.AreEqual("Test.docx", first);
            Assert.AreEqual("Test_1.docx", second);
        }

        [Test]
        public void TestCreateWithExistingFilesCaseSensitive()
        {
            var existingFiles = new[]
                                    {
                                        "test.docx",  
                                        "test_1.docx",
                                        "test_2.docx"
                                    };
            var file = new ImportFile
            {
                Name = "Test.docx",
            };

            var nameResolver = new FlatListDuplicateNameResolver(existingFiles);
            string first = nameResolver.ResolveName(file);
            Assert.AreEqual("Test_3.docx", first);
        }
    }
}