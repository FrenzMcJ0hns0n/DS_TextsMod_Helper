using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS_TextsMod_Helper
{
    public class ProcessingMode
    {
        public List<ReadMode> AllReadModeEntries { get; set; }
        public List<CompareMode> AllCompareModeEntries { get; set; }
        public List<PrepareMode> AllPrepareModeEntries { get; set; }
    }
}
