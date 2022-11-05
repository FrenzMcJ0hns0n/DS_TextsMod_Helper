using System.Collections.Generic;

namespace DS_TextsMod_Helper
{
    public class ProcessingMode
    {
        public List<ReadMode> AllReadModeEntries { get; set; }
        public List<CompareMode> AllCompareModeEntries { get; set; }
        public List<PrepareMode> AllPrepareModeEntries { get; set; }
    }
}
