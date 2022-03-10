using System.IO;
using System.Reflection;

namespace DS_TextsMod_Helper
{
    public class IOHelper
    {

        public static string GetRootDirPath()
        {
            return Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();
        }

        public static string GetOutputDirPath()
        {
            return Path.Combine(GetRootDirPath(), "Output");
        }


        public static string GetCompareOutputFilename(string filename)
        {
            return Path.Combine(GetOutputDirPath(), filename + ".csv");
        }

        public static string GetPrepareOutputFilename(string filename)
        {
            return Path.Combine(GetOutputDirPath(), filename + ".fmg");
        }

    }
}
