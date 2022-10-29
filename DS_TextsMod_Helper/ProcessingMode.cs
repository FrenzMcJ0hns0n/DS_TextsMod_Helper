using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS_TextsMod_Helper
{
    public class ProcessingMode
    {
        public string Title { get; set; }
        public List<ReadMode> AllReadModeEntries { get; set; }
        public List<List<CompareMode.Entry>> AllCompareEntries { get; set; }
        public List<List<PrepareMode.Entry>> AllPrepareEntries { get; set; }
    }
}
