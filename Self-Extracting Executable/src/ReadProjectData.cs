using System.Xml.Linq;

namespace SelfEE
{
    internal class ReadProjectData
    {
        public static string GetProductName(XElement projXml)
        {
            return projXml.Element("Info")?.Element("ApplicationName")?.Value ?? string.Empty;
        }

        public static string GetCompanyName(XElement projXml)
        {
            return projXml.Element("Info")?.Element("CompanyName")?.Value ?? string.Empty;
        }

        public static string GetSaveFolder(XElement projXml)
        {
            return projXml.Element("Info")?.Element("SaveFolder")?.Value ?? string.Empty;
        }

        public static string GetInstaller(XElement projXml)
        {
            return projXml.Element("Installer")?.Element("MainFilePath")?.Value ?? string.Empty;
        }

        public static List<string> GetInstallerFiles(XElement projXml)
        {
            List<string> files = new();
            var elements = projXml.Element("Installer")?.Elements("FilePath")?.ToList();

            if (elements != null)
                foreach (var file in elements)
                    if (file.Value != string.Empty && !files.Contains(file.Value))
                        files.Add(file.Value);
 
            return files;
        }
    }
}
