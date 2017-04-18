using System;
using System.IO;

namespace OrbitOne.SharePoint.Importer.FileSystem
{
    public class InternetShortcutFile
    {
        public string Url { get; set; }
        public string FilePath { get; set; }

        public InternetShortcutFile(string url, string filePath)
        {
            FilePath = filePath;
            Url = url;
        }

        public void Create()
        {
            if (String.IsNullOrEmpty(Url)) throw new ArgumentException("Url can not be empty");
            if (String.IsNullOrEmpty(FilePath)) throw new ArgumentException("FilePath can not be empty");

            if (!File.Exists(FilePath))
            {
                using (var shortcutFile = File.CreateText(FilePath))
                {
                    shortcutFile.WriteLine("[InternetShortcut]");
                    shortcutFile.WriteLine();
                    shortcutFile.WriteLine("URL=" + Url);
                }
            }
        }
    }
}