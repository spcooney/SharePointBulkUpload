using System;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using OrbitOne.SharePoint.Importer.Domain;

namespace OrbitOne.SharePoint.Importer.Tests
{
    [TestFixture]
    public class TestValidation
    {
        [Test]
        public void TestValidatorDefaults()
        {
            var validator = new DefaultValidator();
            var fileToTest = new ImportFile
                                 {
                                     Name = "test.xml",
                                     Size=0
                                 };
            
            var result = validator.Validate(fileToTest);
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void TestDefaultValidatorValidatesBiggerFile()
        {
            var size = 120;
            var validator = new DefaultValidator {MaximumFileSize = size};
            var fileToTest = new ImportFile
            {
                Name = "test.xml",
                Size = size+1
            };

            var result = validator.Validate(fileToTest);
            Assert.IsFalse(result.IsValid);
        }

        [Test]
        public void TestDefaultValidatorValidatesEqualFileSize()
        {
            var size = 120;
            var validator = new DefaultValidator {MaximumFileSize = size};
            var fileToTest = new ImportFile
            {
                Name = "test.xml",
                Size = size
            };

            var result = validator.Validate(fileToTest);
            Assert.IsTrue(result.IsValid);
        }
        [Test]
        public void TestDefaultValidatorValidatesSmallerFileSize()
        {
            var size = 120;
            var validator = new DefaultValidator {MaximumFileSize = size};
            var fileToTest = new ImportFile
            {
                Name = "test.xml",
                Size = size -1
            };

            var result = validator.Validate(fileToTest);
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void TestDefaultValidatorValidatesValidExtension()
        {
            var validator = new DefaultValidator();
            validator.BlockedFileExtensions.Add(".exe");
            var fileToTest = new ImportFile
            {
                Name = "test.xml",
            };

            var result = validator.Validate(fileToTest);
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void TestDefaultValidatorValidatesInvalidExtension()
        {
            var validator = new DefaultValidator();
            validator.BlockedFileExtensions.Add(".xml");
            var fileToTest = new ImportFile
            {
                Name = "test.xml",
            };

            var result = validator.Validate(fileToTest);
            Assert.IsFalse(result.IsValid);
        }


        [Test]
        public void TestDefaultValidatorValidatesFileNameLengthTooLong()
        {
            var validator = new DefaultValidator {MaximumFileNameLength = 120};
            var fileToTest = new ImportFile
            {
                Name = new string('a',120) + ".xml"
            };
            var result = validator.Validate(fileToTest);
            Assert.IsTrue(result.IsValid);
            Assert.That(result.Warnings.Count == 1);
        }

        [Test]
        public void TestDefaultValidatorValidatesFileNameLength()
        {
            var validator = new DefaultValidator {MaximumFileNameLength = 6};
            var fileToTest = new ImportFile
            {
                Name = "e.xml"
            };
            var result = validator.Validate(fileToTest);
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void TestDefaultValidatorValidatesFileNameLengthEqualToMax()
        {
            var validator = new DefaultValidator {MaximumFileNameLength = 5};
            var fileToTest = new ImportFile
            {
                Name = "e.xml"
            };
            var result = validator.Validate(fileToTest);
            Assert.IsTrue(result.IsValid);
        }

        [Test]
        public void TestDefaultValidatorValidatesCharacters()
        {
            var validator = new DefaultValidator {IllegalCharacters = new[] {'a'}};
            var fileToTest = new ImportFile
            {
                Name = "a.xml"
            };
            var result = validator.Validate(fileToTest);
            Assert.IsTrue(result.IsValid);
            Assert.That(result.Warnings.Count == 1);
        }

        [Test]
        public void TestDefaultValidatorValidatesCharactersAllAllowed()
        {
            var validator = new DefaultValidator {IllegalCharacters = new[] {'a'}};
            var fileToTest = new ImportFile
            {
                Name = "b.xml"
            };
            var result = validator.Validate(fileToTest);
            Assert.IsTrue(result.IsValid);
        }
    }
}