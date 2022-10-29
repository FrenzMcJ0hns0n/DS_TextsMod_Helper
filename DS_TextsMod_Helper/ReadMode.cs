using System.Collections.Generic;
using System.IO;
using System.Linq;

using SoulsFormats;

namespace DS_TextsMod_Helper
{
    public class ReadMode
    {
        public string Title { get; set; }
        public string InputFile { get; set; }
        public bool OneLinedValues { get; set; }
        public string OutputFilename { get; set; }
        public char Sep { get; set; }
        public List<ReadEntry> Entries { get; set; }


        public ReadMode(string iFile1)
        {
            InputFile = iFile1;
            Entries = new List<ReadEntry>();
        }


        public string FormatValue(string value)
        {
            if (value == null)
                return value;

            // Preserve white spaces, whatever their purpose
            if (value.Count(char.IsWhiteSpace) == value.Length)
                return value;

            if (OneLinedValues)
                return value.Replace("\n", "/n/").Trim();

            return value.Trim();
        }


        public void ProcessFiles(bool preview)
        {
            // 0. Get input data
            FMG file_1 = new FMG { Entries = FMG.Read(InputFile).Entries };

            Dictionary<int, string> rd_dictionary = new Dictionary<int, string>();

            // 1. Read file
            foreach (FMG.Entry entry in file_1.Entries)
                rd_dictionary.Add(entry.ID, FormatValue(entry.Text));

            // 2. Build Entry
            int index = 0;
            foreach (KeyValuePair<int, string> rd in rd_dictionary)
            {
                index += 1;
                int textId = rd.Key;
                string val = rd.Value;

                Entries.Add(new ReadEntry(index, textId, val));

                if (preview && index == 50) // TODO? v1.4: Give choice about max results
                    break;
            }
        }


        public void ProduceOutput(string oFilename, string csvSepChar)
        {
            OutputFilename = Tools.GetOutputFilepath(oFilename);
            Sep = csvSepChar[0];

            using (StreamWriter writer = new StreamWriter(OutputFilename, false))
            {
                writer.WriteLine($"Text ID{Sep}Value");

                foreach (ReadEntry re in Entries)
                {
                    if (re.Value != null)
                        re.Value = re.Value.Replace("\"", "\"\"");

                    writer.WriteLine($"{re.TextId}{Sep}\"{re.Value}\"");
                    // Generalized usage of double quotes, as it is Excel friendly (TODO? v1.4: Give choice about that)
                }
            }
        }
    }


    public class ReadEntry
    {
        public int Index { get; set; }
        public int TextId { get; set; }
        public string Value { get; set; }

        public ReadEntry(int index, int textId, string value)
        {
            Index = index;
            TextId = textId;
            Value = value;
        }
    }

}
