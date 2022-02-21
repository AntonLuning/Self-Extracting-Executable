using System.IO;
using System.Xml.Linq;

namespace SelfEE
{
    internal class ReadProjectData
    {
        public static string GetProductName(XElement projXml)
        {
            return projXml.Element("Info")?.Element("Application")?.Element("Name")?.Value ?? string.Empty;
        }

        public static string GetProductVersion(XElement projXml)
        {
            var version = projXml.Element("Info")?.Element("Application")?.Element("Version")?.Value ?? string.Empty;
            if (version == null)
            {
                Log.WriteWarning(string.Format("Project file ({0}) does not contain Version in 'Info-Application'. The product version will be set to 1,0,0,0.", projXml.Document?.BaseUri));
                return "1,0,0";
            }

            return version.Replace('.', ',');
        }

        public static string GetCompanyName(XElement projXml)
        {
            return projXml.Element("Info")?.Element("Application")?.Element("CompanyName")?.Value ?? string.Empty;
        }

        public static string GetSaveFolder(XElement projXml)
        {
            return projXml.Element("Info")?.Element("SFX")?.Element("SaveFolder")?.Value ?? string.Empty;
        }

        public static string GetSetupIconPath(XElement projXml)
        {
            return projXml.Element("Info")?.Element("SFX")?.Element("SetupIcon")?.Value ?? string.Empty;
        }

        public static List<string> GetInstallerFiles(XElement projXml)
        {
            var folders = projXml.Element("Installer")?.Elements("FolderPath")?.ToList();

            List<string> installerFiles = new();

            if (folders != null)
                foreach (var folder in folders)
                    if (folder.Value != string.Empty && Directory.Exists(folder.Value))
                    {
                        var files = Directory.GetFiles(folder.Value);
                        foreach (var file in files)
                            if(!installerFiles.Contains(file))
                                installerFiles.Add(file);
                    }
 
            return installerFiles;
        }

        public static string GetMainInstallerName(XElement projXml)
        {
            return projXml.Element("Installer")?.Element("MainFileName")?.Value ?? string.Empty;
        }
    }
}
