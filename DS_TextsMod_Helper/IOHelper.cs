using System.IO;
using System.Reflection;

namespace DS_TextsMod_Helper
{
    public class IOHelper
    {

        public static string ReturnRootDirectoryPath()
        {
            return Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();
        }

        public static string ReturnOutputDirectoryPath()
        {
            return Path.Combine(ReturnRootDirectoryPath(), "Output");
        }


        public static string ReturnCompareOutputFilename(string filename)
        {
            return Path.Combine(ReturnOutputDirectoryPath(), filename + ".csv");
        }

        public static string ReturnPrepareOutputFilename(string filename)
        {
            return Path.Combine(ReturnOutputDirectoryPath(), filename + ".fmg");
        }

    }
}
