using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS_TextsMod_Helper
{
    class InputFileDTO
    {
        public string Filename { get; set; }
        public string Type { get; set; }


        public InputFileDTO(string filename, string fmgVersion)
        {
            Filename = filename;
            Type = ShortenVersion(fmgVersion);
        }

        private string ShortenVersion(string fmgVersion)
        {
            switch (fmgVersion)
            {
                case "Demon's Souls":
                    return "DeS";

                case "Dark Souls 1 / Dark Souls 2":
                    return "DkS 1|2";

                case "Dark Souls 3 / Bloodborne":
                    return "DkS 3 / BB";

                default:
                    return "DkS 1|2";
            }
        }
    }
}
