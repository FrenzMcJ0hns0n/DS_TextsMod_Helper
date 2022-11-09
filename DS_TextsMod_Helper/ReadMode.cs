using System.Collections.Generic;
using System.IO;
using System.Linq;

using SoulsFormats;

namespace DS_TextsMod_Helper
{
    public class ReadMode
    {
        public List<string> Errors { get; set; }
        public string InputFile { get; set; }
        public bool OneLinedValues { get; set; }
        public string OutputFilename { get; set; }
        public char Sep { get; set; }
        public string Title { get; set; }
        public List<ReadEntry> Entries { get; set; }


        public ReadMode(string iFileA)
        {
            InputFile = iFileA;
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
            Errors = new List<string>();
            Dictionary<int, string> rdDictionary = new Dictionary<int, string>();

            // 0. Get input data
            FMG fileA = new FMG { Entries = FMG.Read(InputFile).Entries };

            // 1. Read file
            int count = 0;
            foreach (FMG.Entry entry in fileA.Entries)
            {
                count += 1;
                if (rdDictionary.ContainsKey(entry.ID))
                {
                    Errors.Add(
                        $"Unicity constraint error. Skipped entry ID {entry.ID} since already registered from input file. Associated Text = \r\n" +
                        $"    {entry.Text}"
                    );
                    continue;
                }
                rdDictionary.Add(entry.ID, FormatValue(entry.Text));

                if (preview && count == 50) // TODO? v1.6: Give choice about max results in Preview
                    break;
            }

            // 2. Build Entry
            foreach (KeyValuePair<int, string> rd in rdDictionary)
            {
                Entries.Add(new ReadEntry(
                    rd.Key,
                    rd.Value
                ));
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
                    // Generalized usage of double quotes, as it is Excel friendly (IDEA? Give choice about that)
                }
            }
        }
    }


    public class ReadEntry
    {
        public int TextId { get; set; }
        public string Value { get; set; }

        public ReadEntry(int textId, string value)
        {
            TextId = textId;
            Value = value;
        }
    }

}
