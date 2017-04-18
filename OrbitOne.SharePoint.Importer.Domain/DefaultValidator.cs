using System;
using System.Linq;
using System.Collections.Generic;

namespace OrbitOne.SharePoint.Importer.Domain
{
    public class DefaultValidator : IImportValidator
    {
        public DefaultValidator(long maximumFileSize, IEnumerable<string> blockedFileExtensions)
        {
            IllegalCharacters = new char[]{};
            MaximumFileNameLength = 123;
            MaximumRelativePathLength = 260;
            MaximumFileSize = maximumFileSize;
            BlockedFileExtensions = blockedFileExtensions.ToList();
        }

        public DefaultValidator()
            : this(long.MaxValue, Enumerable.Empty<string>())
        { }

        public long MaximumFileSize { get; set; }
        public IList<string> BlockedFileExtensions { get; set; }
        public int MaximumFileNameLength { get; set; }
        public int MaximumRelativePathLength { get; set; }
        public char[] IllegalCharacters { get; set; }
        public bool DocumentLibraryExists { get; set; }
        public bool IsValid
        {
            get
            {
                return DocumentLibraryExists;
            }
        }

        public ValidationResult Validate(ImportFile file)
        {
            var result = new ValidationResult{Source=file.OriginalFullName};
            if (ExtensionIsBlocked(file.Extension))
            {
                result.AddError("extension is blocked");
            }
            if (!NameIsValid(file.Name))
            {
                result.AddWarning("Filename is invalid");
            }
            if (file.Size > MaximumFileSize)
            {
                result.AddError("File is too big");
            }
            if (file.ServerRelativePath.Length > MaximumRelativePathLength)
            {
                result.AddWarning("File path is too long");
            }
            
            return result;
        }

        private bool ExtensionIsBlocked(string extension)
        {
            return BlockedFileExtensions.Any(ext => ext == extension);
        }
       
        private bool NameIsValid(string name)
        {
            return
                CharactersAreValid(name)
                && name.Length <= MaximumFileNameLength
                && !(name.StartsWith(".") || name.EndsWith(".") || name.Contains(".."));
        }

        public ValidationResult Validate(ImportFolder directory)
        {
            var result = new ValidationResult{Source=directory.OriginalFullName};
            if (!NameIsValid(directory.Name))
            {
                result.AddWarning("Directory name is invalid");
            }
            if (directory.ServerRelativePath.Length > MaximumRelativePathLength)
            {
                result.AddError("Full directory name is too long. Maximum length is " + MaximumRelativePathLength);
            }
            if (directory.Name == "bin")
            {
                result.AddError("Directory name is blocked: " + directory.Name);
            }
            return result;
        }

        private bool CharactersAreValid(string filename)
        {
            return filename.IndexOfAny(IllegalCharacters) == -1;
        }
    }
}