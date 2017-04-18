using System;
using System.IO;

namespace OrbitOne.SharePoint.Importer.FileSystem
{
    public class FileNameConverter 
    {
        public char[] IllegalCharacters { get; set; }
        public int MaximumFileNameLenght { get; set; }

        public string Convert(string filename)
        {
            string cleaned = filename;
            cleaned = StripIllegalCharacters(cleaned);
            cleaned = Shorten(cleaned);
            return cleaned;
        }

        private string StripIllegalCharacters(string filename)
        {
            var cleaned = filename.Trim('.', ' ');
            foreach(var c in IllegalCharacters)
            {
                cleaned = cleaned.Replace(c.ToString(), string.Empty);
            }
            while(cleaned.Contains(".."))
            {
                cleaned = cleaned.Replace("..", ".");
            }
            return cleaned;
        }

        private string Shorten(string filename)
        {
            if(filename.Length > MaximumFileNameLenght)
            {
                var name = Path.GetFileNameWithoutExtension(filename);
                var extension = Path.GetExtension(filename);
                
                return String.Concat(
                    name.Substring(0, MaximumFileNameLenght - extension.Length),
                    extension
                    );
            }
            return filename;
        }
    }
}