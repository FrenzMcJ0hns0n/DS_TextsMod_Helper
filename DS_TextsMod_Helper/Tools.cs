using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace DS_TextsMod_Helper
{
    public class Tools
    {

        #region System

        public static string GetFormattedAppVersion()
        {
            AssemblyName assemblyName = Assembly.GetExecutingAssembly().GetName();

            bool hasMinor = assemblyName.Version.Minor > 0;
            bool hasBuild = assemblyName.Version.Build > 0;
            bool hasRevis = assemblyName.Version.Revision > 0;

            string major = assemblyName.Version.Major.ToString();
            string minor = hasMinor || hasBuild || hasRevis ? $".{assemblyName.Version.Minor}" : "";
            string build = hasBuild || hasRevis ? $".{assemblyName.Version.Build}" : "";
            string revis = hasRevis ? $".{assemblyName.Version.Revision}" : "";

            return $"DS Texts Mod Helper - v{major}{minor}{build}{revis}";
        }

        #endregion


        #region Logging

        public static void LogProcessingError(string procMode, List<string> errors, string iFileA, string iFileB = "", string iFileC = "")
        {
            string errLogFile = Path.Combine(GetOutputDirPath(), GetFileName(iFileA, false) + "_error.txt");
            using (StreamWriter writer = new StreamWriter(errLogFile, true))
            {
                writer.WriteLine($"{DateTime.Now} - Error while processing files in {procMode} mode. Details :");
                foreach (string err in errors)
                {
                    writer.WriteLine($"- {err}");
                }
                writer.WriteLine();
            }
        }

        #endregion


        #region IO shortcuts

        public static string GetRootDirPath()
        {
            return Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();
        }

        public static string GetOutputDirPath()
        {
            return Path.Combine(GetRootDirPath(), "Output");
        }

        public static string GetOutputFilepath(string filename)
        {
            return Path.Combine(GetOutputDirPath(), filename);
        }

        public static string GetSoulsFormatsDllPath()
        {
            return Path.Combine(GetRootDirPath(), "SoulsFormats.dll");
        }

        #endregion


        #region Input files properties

        public static string GetFileName(string path, bool keepExtension)
        {
            FileInfo info = new FileInfo(path);
            string fileName = info.Name;

            return keepExtension ? fileName : fileName.Replace(info.Extension, "");
        }

        public static long GetFileSize(string path)
        {
            return new FileInfo(path).Length;
        }

        public static string GetParentDirPath(string path)
        {
            FileInfo info = new FileInfo(path);
            string parentFolder = info.DirectoryName;

            return parentFolder;
        }

        private static int GetSecondsSinceLastEdit(string path)
        {
            DateTime lastWriteTime = new FileInfo(path).LastWriteTime;
            DateTime loadedNow = DateTime.Now;

            return (int)(loadedNow - lastWriteTime).TotalSeconds;
        }

        public static string GetTimeSinceLastEdit(string path)
        {
            int seconds = GetSecondsSinceLastEdit(path);

            if (seconds > 60 * 60 * 24)
                return $"{seconds / (60 * 60 * 24)} day(s)";

            else if (seconds > 60 * 60)
                return $"{seconds / (60 * 60)} hour(s)";

            else return $"{seconds / 60} minute(s)";
        }

        #endregion


        #region Output preview style

        public static double GetColumnMaxWidth()
        {
            return SystemParameters.FullPrimaryScreenWidth >= 2500 ? 540 : 360;
        }

        #endregion


        #region Misc. output operations

        public static List<string> GetAlreadyExistingFilenames(List<string> iFilenames)
        {
            DirectoryInfo di = new DirectoryInfo(GetOutputDirPath());
            List<string> oFilenames = di.GetFiles().Select(fi => fi.Name).ToList();

            return iFilenames.Where(iFile => oFilenames.Contains(iFile)).ToList();
        }

        #endregion

    }
}
