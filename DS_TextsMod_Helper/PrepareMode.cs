using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DS_TextsMod_Helper
{
    internal class PrepareMode : IProcessingModes
    {

        public List<string> InputFiles { get; set; }
        public string TrimBegin { get; set; }
        public string TrimEnd { get; set; }
        public string OutputFilename { get; set; }
        public bool OneLinedPreview { get; set; }
        public List<Entry> Entries { get; set; }


        public PrepareMode(string iFile1, string iFile2, string iFile3) // TODO: Add TrimBegin & TrimEnd
        {
            InputFiles = new List<string>() { iFile1, iFile2, iFile3 };
            //OutputFilename = oFilename;
            //Entries = new List<Entry>();
        }


        public string FormatValue(string value)
        {
            if (value == " ")
                return value;

            if (string.IsNullOrEmpty(TrimBegin))
                return value.Trim();

            if(value.Contains(TrimBegin))
                return value.Replace(TrimBegin, TrimEnd).Trim();

            return value.Trim();
        }


        public void ProcessFiles(bool preview)
        {
            Entries = new List<Entry>();

            // Get input data
            SoulsFormats.FMG file_1 = new SoulsFormats.FMG { Entries = SoulsFormats.FMG.Read(InputFiles[0]).Entries };
            SoulsFormats.FMG file_2 = new SoulsFormats.FMG { Entries = SoulsFormats.FMG.Read(InputFiles[1]).Entries };
            SoulsFormats.FMG file_3 = new SoulsFormats.FMG { Entries = SoulsFormats.FMG.Read(InputFiles[2]).Entries };

            Dictionary<int, List<string>> prp_dictionary = new Dictionary<int, List<string>>();

            // 1. Take all values from File1
            int counter = 0;
            foreach (SoulsFormats.FMG.Entry entry in file_1.Entries)
            {
                if (entry.Text == null)
                    prp_dictionary.Add(entry.ID, new List<string>() { "", "", "" }); // Preserve null values. TODO: Test "" vs. null

                entry.Text = FormatValue(entry.Text);

                prp_dictionary.Add(entry.ID, new List<string>() { entry.Text, "", "" }); // Create entry, so far with File1 value

                counter += 1;
                if (preview && counter == 30)
                    break;
            }

            // 2. Insert values from File2 for matching entry.ID
            foreach (SoulsFormats.FMG.Entry entry in file_2.Entries)
            {
                if (entry.Text == null)
                    continue;

                entry.Text = FormatValue(entry.Text);

                if (prp_dictionary.ContainsKey(entry.ID))
                    prp_dictionary[entry.ID][1] = entry.Text; // Insert File2 value
            }

            // 3. Insert values from File3 for matching entry.ID
            foreach (SoulsFormats.FMG.Entry entry in file_3.Entries)
            {
                if (entry.Text == null)
                    continue;

                entry.Text = FormatValue(entry.Text);

                if (prp_dictionary.ContainsKey(entry.ID))
                    prp_dictionary[entry.ID][2] = entry.Text; // Insert File3 value
                // else : File3 value ignored as not in File1 structure
            }


            int index = 0;
            foreach (KeyValuePair<int, List<string>> prp in prp_dictionary)
            {
                index += 1;
                int textId = prp.Key;
                string val1 = prp.Value[0];
                string val2 = prp.Value[1];
                string val3 = prp.Value[2];
                string output = val1 == val2 ? val3 : val1;
                string source = val1 == val2 ? "File #3" : "File #1";

                Entries.Add(new Entry(index, textId, val1, val2, val3, output, source));
            }
        }


        public class Entry
        {
            public int Index { get; set; }
            public int TextId { get; set; }
            public string Value1 { get; set; }
            public string Value2 { get; set; }
            public string Value3 { get; set; }
            public string Output { get; set; }
            public string Source { get; set; }

            public Entry(int index, int textId, string value1, string value2, string value3, string output, string source)
            {
                Index = index;
                TextId = textId;
                Value1 = value1;
                Value2 = value2;
                Value3 = value3;
                Output = output;
                Source = source;
            }
        }
    }
}
