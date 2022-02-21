using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SelfEE
{
    internal class SFX
    {
        private const string COMPRESSED_FILE = "compressed.7z";
        private const string CONFIG_FILE = "SFXconfig.txt";
        private readonly string _baseDir;
        private readonly string _sevenZipEXE;
        private readonly string _sevenZipSFX;
        private readonly string _resourceHacker;

        private string _productName;
        private string _saveFolder;
        private string _executablePath;

        private List<string> _installerFiles;
        private string _mainInstallerPath;

        private List<string> _programFiles;
        private List<string> _applicationFiles;

        public SFX()
        {
            _baseDir = AppDomain.CurrentDomain.BaseDirectory;
            if (!_baseDir.Contains("bin"))
                throw new EntryPointNotFoundException("Base directory is not located inside of bin directory.");

            _sevenZipEXE = Path.Combine(_baseDir.Split("bin")[0], @"Self-Extracting Executable\vendor\7z\7za.exe");
            if (!File.Exists(_sevenZipEXE))
                throw new FileNotFoundException("File could not be found.", _sevenZipEXE);

            _sevenZipSFX = Path.Combine(_baseDir.Split("bin")[0], @"Self-Extracting Executable\vendor\7z\7zS.sfx");
            if (!File.Exists(_sevenZipSFX))
                throw new FileNotFoundException("File could not be found.", _sevenZipSFX);

            _resourceHacker = Path.Combine(_baseDir.Split("bin")[0], @"Self-Extracting Executable\vendor\Resource Hacker\ResourceHacker.exe");
            if (!File.Exists(_resourceHacker))
                throw new FileNotFoundException("File could not be found.", _resourceHacker);
       
            File.Delete(COMPRESSED_FILE);
            File.Delete(CONFIG_FILE);
        }

        public bool Generate(XElement projXml)
        {
            _productName = ReadProjectData.GetProductName(projXml);
            if (_productName == string.Empty)
            {
                Log.WriteError(String.Format("Project file ({0}) must contain Name in 'Info-Application'.", projXml.Document?.BaseUri));
                return false;
            }

            _saveFolder = ReadProjectData.GetSaveFolder(projXml);
            if (_saveFolder == string.Empty)
            {
                Log.WriteError(String.Format("Project file ({0}) must contain SaveFolder in 'Info-SFX'.", projXml.Document?.BaseUri));
                return false;
            }

            _executablePath = Path.Combine(_saveFolder, _productName + "_Setup.exe");
            File.Delete(_executablePath);

            _installerFiles = ReadProjectData.GetInstallerFiles(projXml);
            if (_installerFiles.Count == 0)
            {
                Log.WriteError(String.Format("Project file ({0}) must contain a FolderPath in 'Installer' with the installer files.", projXml.Document?.BaseUri));
                return false;
            }

            string insName = ReadProjectData.GetMainInstallerName(projXml);
            if (insName == string.Empty)
            {
                Log.WriteError(String.Format("Project file ({0}) must contain a MainFileName in 'Installer'.", projXml.Document?.BaseUri));
                return false;
            }

            foreach (var filePath in _installerFiles)
                if (Path.GetFileName(filePath) == insName)
                {
                    _mainInstallerPath = filePath;
                    break;
                }
            if (_mainInstallerPath == string.Empty)
            {
                Log.WriteError(String.Format("The MainFileName in 'Installer' (in {0}) must be located in the directory FolderPath in 'Installer'.", projXml.Document?.BaseUri));
                return false;
            }

            if (!ZipApplicationFiles())
            {
                Log.WriteError("Failed to zip application files. Could not run the 7-zip program.");
                return false;
            }

            CreateArchive(CONFIG_FILE, Path.GetFileName(_mainInstallerPath), false, "Run XCC Setup");
            Thread.Sleep(500);    // To make sure that the files are released before the upcoming process

            if (!CreateSFX(projXml))
            {
                Log.WriteError("Failed to generate the self-extracting executable.");
                return false;
            }

            try
            {
                Thread.Sleep(500);     // To avoid "Used in other process" error when trying to delete following files
                File.Delete(COMPRESSED_FILE);
                File.Delete(CONFIG_FILE);
            }
            catch (Exception)
            {
                Log.WriteWarning("Failed to delete intermediate files.");
            }

            return true;
        }
    
        private bool ZipApplicationFiles()
        {
            if (!Processes.Run7Zip(_sevenZipEXE, _installerFiles.ToArray(), COMPRESSED_FILE))
                return false;

            Log.WriteInfo("Files (and folders) zipped.");
            return true;
        }

        private void CreateArchive(string filePath, string runFile, bool showProgressBar, string title)
        {
            string showProgressBarOnExtraction = showProgressBar ? "yes" : "no";

            using (FileStream fs = new(filePath, FileMode.Create, FileAccess.ReadWrite))
            {
                string text = string.Format(";!@Install@!UTF-8! Title=\"{1}\" BeginPrompt=\"{0}\" Progress=\"{2}\" RunProgram=\"{3}\";!@InstallEnd@!",
                    "Would you like to extract and run the setup?",
                    title,
                    showProgressBarOnExtraction,
                    runFile);

                byte[] byteText = Encoding.UTF8.GetBytes(text);
                fs.Write(byteText, 0, byteText.Length);
                fs.Flush();
            }

            Log.WriteInfo("Extraction program generated.");
        }

        private bool CreateSFX(XElement projXml)
        {
            if (!Processes.RunCommand(string.Format("/C copy /b \"{0}\" + \"{1}\" + \"{2}\" \"{3}\"", _sevenZipSFX, CONFIG_FILE, COMPRESSED_FILE, _executablePath)))
                return false;

            string versionInfo = VersionInfo.GenerateResFile(projXml, _baseDir);
            Thread.Sleep(500);    // To make sure that the files are released before the upcoming process

            Processes.RunCommands(new string[] {
                string.Format("cd \"{0}\"", _baseDir),
                string.Format("\"{0}\" -open {1}.rc -save {1}.res -action compile -log con", _resourceHacker, versionInfo),
                string.Format("\"{0}\" -open \"{1}\" -save \"{1}\" -res {2}.res -action addoverwrite -mask VersionInfo,, -log con", _resourceHacker, _executablePath, versionInfo)
            });

            string iconPath = ReadProjectData.GetSetupIconPath(projXml);

            Processes.RunCommands(new string[] {
                string.Format("cd \"{0}\"", _baseDir),
                string.Format("\"{0}\" -open \"{1}\" -save \"{1}\" -action addoverwrite -res \"{2}\" -mask ICONGROUP, MAINICON, 0", _resourceHacker, _executablePath, iconPath),
                "ie4uinit.exe -ClearIconCache"
            });

            Log.WriteInfo("Self-extracting executable generated.");
            return true;
        }
    }
}
