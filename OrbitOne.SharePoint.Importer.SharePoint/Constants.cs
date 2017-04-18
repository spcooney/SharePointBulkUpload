using System;

namespace OrbitOne.SharePoint.Importer.SharePoint
{
    public class Constants
    {
        public static readonly char[] IllegalCharacters = new[] { '"', '#', '%', '&', '*', ':', '<', '>', '?', '\\', '/', '{', '|', '}', '~' };
        public static readonly String[] BlockedFileExtensions = new[] { ".ade", ".adp", ".app", ".asa", ".ashx", ".asmx", ".asp", ".bas", ".bat", ".cdx", ".cer", ".chm", ".class", ".cmd", ".cnt", ".com", ".config", ".cpl", ".crt", ".csh", ".der", ".dll", ".exe", ".fxp", ".gadget", ".hlp", ".hpj", ".hta", ".htr", ".htw", ".ida", ".idc", ".idq", ".ins", ".isp", ".its", ".jse", ".ksh", ".lnk", ".mad", ".maf", ".mag", ".mam", ".maq", ".mar", ".mas", ".mat", ".mau", ".mav", ".maw", ".mda", ".mdb", ".mde", ".mdt", ".mdw", ".mdz", ".msc", ".msh", ".msh1", ".msh1xml", ".msh2", ".msh2xml", ".mshxml", ".msi", ".msp", ".mst", ".ops", ".pcd", ".pif", ".prf", ".prg", ".printer", ".ps1", ".ps1xml", ".ps2", ".ps2xml", ".psc1", ".psc2", ".pst", ".reg", ".rem", ".scf", ".scr", ".sct", ".shb", ".shs", ".shtm", ".shtml", ".soap", ".stm", ".svc", ".url", ".vb", ".vbe", ".vbs", ".ws", ".wsc", ".wsf", ".wsh" };
        public static readonly String[] InvalidFileExtensions = new[] { ".cs", ".csproj" };
        public static readonly String[] InvalidFolderNames = new[] { "bin"};
    }
}
