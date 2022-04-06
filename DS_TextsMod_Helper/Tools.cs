﻿using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace DS_TextsMod_Helper
{
    public class Tools
    {

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




        public static string GetFileName(string path)
        {
            FileInfo info = new FileInfo(path);
            string fileName = info.Name;
            string fileExt = info.Extension;

            return fileName.Replace(fileExt, "");
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




        public static string GetTimeSinceLastEdit(string path)
        {
            int seconds = GetSecondsSinceLastEdit(path);

            if (seconds > 60 * 60 * 24)
                return $"{seconds / (60 * 60 * 24)} day(s)";

            else if (seconds > 60 * 60)
                return $"{seconds / (60 * 60)} hour(s)";

            else return $"{seconds / 60} minute(s)";
        }

        private static int GetSecondsSinceLastEdit(string path)
        {
            DateTime lastWriteTime = new FileInfo(path).LastWriteTime;
            DateTime loadedNow = DateTime.Now;

            return (int)(loadedNow - lastWriteTime).TotalSeconds;
        }




        public static double GetColumnMaxWidth()
        {
            return SystemParameters.FullPrimaryScreenWidth >= 2500 ? 540 : 360;
        }

    }
}
