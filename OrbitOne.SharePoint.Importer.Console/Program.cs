using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using log4net;
using OrbitOne.SharePoint.Importer.CommandLineParsing;
using OrbitOne.SharePoint.Importer.Domain;

namespace OrbitOne.SharePoint.Importer.CommandLine
{
    class Program
    {
        private static ILog log = LogManager.GetLogger(typeof(Program));

        static void Main(string[] args)
        {
            InitLogging();
            var settings = GetSettings(args);
            
            if (!Verify(settings))
            {
                PrintUsage();
                log.Error("Settings are not valid, Import aborted.");
            }
            else
            {
                var importer = new DocumentImporter(settings);
                importer.Execute();
            }
            log.Info("Import completed.");
        }

        private static void PrintUsage()
        {
            Console.WriteLine("Required Arguments:");
            Console.WriteLine();
            Console.WriteLine("-folder\t\t The directory that you want to import.");
            Console.WriteLine("-site\t\t The url of the site you cwant to import to.");
            Console.WriteLine("-documentlibrary\t The name of the document library you want to import to");
            Console.WriteLine();
            Console.WriteLine("Optional arguments");
            Console.WriteLine();
            Console.WriteLine("-CreateFolders:\t\t If present, folders will be created in the target\n\t\t\t document library. If not, all files will be imported\n\t\t\t to the root.");
            Console.WriteLine("-ImportHiddenFiles\t If present, hidden files will be imported.\n\t\t\t By default the are skipped.");
            Console.WriteLine("-CreateEmptyFolders\t If present, empty folders will be created in the\n\t\t\t target document library. By default they are skipped.");
            Console.WriteLine("-MovedFolder\t\t If present, all imported files will be moved to the\n\t\t\t specified folder");
        }

        private static void InitLogging()
        {
            if (true)
            {
                log4net.Config.XmlConfigurator.Configure();
            }
        }

        private static bool Verify(ImportSettings settings)
        {
            bool valid = true;
            if (settings == null)
            {
                log.Error("Settings are null");
                return false;
            }
            if (String.IsNullOrEmpty(settings.DocumentLibrary))
            {
                log.Error("documentlibrary is required");
                valid = false;
            }
            if (String.IsNullOrEmpty(settings.SiteUrl))
            {
                log.Error("site url is required");
                valid = false;
            }
            if (String.IsNullOrEmpty(settings.SourceFolder))
            {
                log.Error("folder is required");
                valid = false;
            }
            else
            {
                var source = new DirectoryInfo(settings.SourceFolder);
                if (!source.Exists)
                {
                    log.Error("Source folder does not exist: " + settings.SourceFolder);
                    valid = false;
                }
            }

            if (settings.MoveFiles)
            {
                var dir = new DirectoryInfo(settings.ArchiveFolder);
                if (!dir.Exists)
                {
                    log.Error("Target directory does not exist: " + settings.ArchiveFolder);
                    valid = false;
                }
                else if (dir.GetFileSystemInfos().Length > 0)
                {
                    log.Error("Target directory is not empty: " + settings.ArchiveFolder);
                    valid = false;
                }
            }
            return valid;
        }

        private static ImportSettings GetSettings(IEnumerable<string> args)
        {
            var settings = new ImportSettings();
            try
            {
                var options = new CommandLineArguments();
                options.ParseArguments(args, '-', ':');
                settings.DocumentLibrary = options.documentlibrary;
                settings.SiteUrl = options.site;
                settings.SourceFolder = options.folder;
                settings.ArchiveFolder = options.Archive;
                settings.CreateFolders = options.CreateFolders;
                settings.ImportHiddenFiles = options.ImportHiddenFiles;
                settings.CreateEmptyFolders = options.CreateEmptyFolders;
                settings.LoggingEnabled = !options.nolog;
                settings.Username = options.Username;
                settings.Password = options.Password;
                settings.Domain = options.Domain;

                if (options.WhatIf)
                {
                    settings.Mode = ImportMode.WhatIf;
                }
                else if (options.Analyse)
                {
                    settings.Mode = ImportMode.Analyse;
                }
                else
                {
                    settings.Mode = ImportMode.Execute;
                }

                if (!string.IsNullOrEmpty(options.authenticationmode) && Enum.IsDefined(typeof(AuthenticationMode), options.authenticationmode))
                {
                    settings.AuthenticationMode = (AuthenticationMode)Enum.Parse(typeof(AuthenticationMode), options.authenticationmode, true);
                }
                else {
                    settings.AuthenticationMode = AuthenticationMode.Windows;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return settings;
        }
    }
}
