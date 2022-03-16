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



        public static string GetFilenameFromPath(string path)
        {
            FileInfo info = new FileInfo(path);
            string fileName = info.Name;
            string fileExt = info.Extension;

            return fileName.Replace(fileExt, "");
        }

        public static string GetParentFolder(string path)
        {
            FileInfo info = new FileInfo(path);
            string parentFolder = info.DirectoryName;

            return parentFolder;
        }

        public static double GetColumnMaxWidth()
        {
            return SystemParameters.FullPrimaryScreenWidth >= 2500 ? 540 : 360;
        }

    }
}
