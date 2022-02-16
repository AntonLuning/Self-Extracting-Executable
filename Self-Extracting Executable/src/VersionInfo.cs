﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SelfEE
{
    internal class VersionInfo
    {
        private const string FILE_NAME = "resource";

        public static string GenerateResFile(XElement projXml, string baseDir)
        {
            string fileVersion = "1,0,0,1";     // temp
            string productVersion = "1,0,0,2";  // temp

            string filePath = Path.Combine(baseDir, FILE_NAME + ".rc");

            using (StreamWriter sw = File.CreateText(filePath))
            {
                sw.WriteLine("");
                sw.WriteLine("1 VERSIONINFO");
                sw.WriteLine("FILEVERSION " + fileVersion);
                sw.WriteLine("PRODUCTVERSION " + productVersion);
                sw.WriteLine("FILEOS 0x4");
                sw.WriteLine("FILETYPE 0x1");
                sw.WriteLine("{");
                sw.WriteLine("BLOCK \"StringFileInfo\"");
                sw.WriteLine("{");
                sw.WriteLine("\tBLOCK \"040904b0\"");
                sw.WriteLine("\t{");
                sw.WriteLine("\t\tVALUE \"CompanyName\", \"" + ReadProjectData.GetCompanyName(projXml) + "\"");
                sw.WriteLine("\t\tVALUE \"FileDescription\", \"Setup program for " + ReadProjectData.GetProductName(projXml) + "\"");
                sw.WriteLine("\t\tVALUE \"FileVersion\", \"" + fileVersion + "\"");
                sw.WriteLine("\t\tVALUE \"ProductName\", \"" + ReadProjectData.GetProductName(projXml) + "_Setup\"");
                sw.WriteLine("\t\tVALUE \"ProductVersion\", \"" + productVersion + "\"");
                sw.WriteLine("\t}");
                sw.WriteLine("}");
                sw.WriteLine("");
                sw.WriteLine("BLOCK \"VarFileInfo\"");
                sw.WriteLine("{");
                sw.WriteLine("\tVALUE \"Translation\", 0x0409 0x04B0");
                sw.WriteLine("}");
                sw.WriteLine("}");
            }

            return FILE_NAME;
        }
    }
}
