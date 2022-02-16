using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfEE
{
    internal class Processes
    {
        public static bool Run7Zip(string sevenZipExe, string[] files, string outputFile)
        {
            Process process = new();
            process.StartInfo.FileName = sevenZipExe;
            process.StartInfo.Arguments = $"a \"{outputFile}\" {PrintFiles(files)}";
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;

            bool result = process.Start();

            return result;
        }

        public static bool RunCommand(string command)
        {
            Process process = new();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = command;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;

            bool result = process.Start();
            process.WaitForExit();

            return result;
        }

        public static void RunCommands(string[] commands)
        {
            Process process = new();
            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.CreateNoWindow = false;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;

            process.Start();

            foreach (string cmd in commands)
            {
                process.StandardInput.WriteLine(cmd);
                process.StandardInput.Flush();
            }
            process.StandardInput.Close();
            process.WaitForExit();
        }

        private static string PrintFiles(string[] files)
        {
            string result = string.Empty;
            foreach (string file in files)
                result += $"\"{file}\" ";

            return result.TrimEnd(' ');
        }
    }
}
