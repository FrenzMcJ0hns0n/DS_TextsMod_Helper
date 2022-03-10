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

    }
}
