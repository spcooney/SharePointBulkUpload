using System.IO;
using NUnit.Framework;

namespace OrbitOne.SharePoint.Importer.Tests
{
    [TestFixture]
    public class TestFileLoading
    {
        [Test]
        public void TestcombinePath()
        {
            string expected = @"c:\temp\imported\abc\blah\file.txt";
            string folder = @"c:\temp\imported";
            string file = @"abc\blah\file.txt";
            Assert.AreEqual(expected,Path.Combine(folder,file));
        }
    }
}