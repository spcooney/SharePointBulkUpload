namespace OrbitOne.SharePoint.Importer.Domain
{
    public interface IImportValidator
    {
        ValidationResult Validate(ImportFile file);
        ValidationResult Validate(ImportFolder directory);
        int MaximumFileNameLength { get; }
        char[] IllegalCharacters { get; set; }
        bool DocumentLibraryExists { get;set;  }
        bool IsValid { get; }
    }
}