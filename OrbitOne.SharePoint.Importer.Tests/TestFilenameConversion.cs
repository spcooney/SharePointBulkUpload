using System.Linq;
using NUnit.Framework;
using OrbitOne.SharePoint.Importer.FileSystem;

namespace OrbitOne.SharePoint.Importer.Tests
{
    [TestFixture]
    public class TestFilenameConversion
    {
        private FileNameConverter m_converter;
        private readonly char[] m_illegalCharacters = new [] {'#','%','&','*',':','<','>','?','\\','/','{','|','}','~'};

        [SetUp]
        public void Setup()
        {
            m_converter = new FileNameConverter{MaximumFileNameLenght = 123, IllegalCharacters = m_illegalCharacters};
        }

        [Test]
        public void TestConvertNameWithMultipleConsecutiveDots()
        {
            
            string original = "test..doc";
            string cleaned = m_converter.Convert(original);
            Assert.AreEqual("test.doc",cleaned);
        }

        [Test]
        public void TestConvertNameWithIllegalCharacters()
        {
            var invalidNames = m_illegalCharacters.Select(c => c + "test.doc");
            foreach (var invalidName in invalidNames)
            {
                string cleaned = m_converter.Convert(invalidName);
                Assert.AreEqual("test.doc", cleaned);
            }
        }

        [Test]
        public void TestFilenameTooLongIsTruncated()
        {
            m_converter.MaximumFileNameLenght = 10;
            var tooLongFileName = "12345678910.doc";
            Assert.AreEqual("123456.doc",m_converter.Convert(tooLongFileName));
        }

        
    }
}