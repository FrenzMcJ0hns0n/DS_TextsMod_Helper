using System.Collections.Generic;
using System.IO;
using System.Linq;

using SoulsFormats;

namespace DS_TextsMod_Helper
{
    public class CompareMode
    {
        public List<CompareEntry> Entries { get; set; }
        public List<string> Errors { get; set; }
        public List<string> InputFiles { get; set; }
        public bool OneLinedValues { get; set; }
        public string OutputFilename { get; set; }
        public string OutputHeaderA { get; set; }
        public string OutputHeaderB { get; set; }
        public char Sep { get; set; }
        public string Title { get; set; }



        public CompareMode(string iFile1, string iFile2)
        {
            InputFiles = new List<string>() { iFile1, iFile2 };
            Entries = new List<CompareEntry>();
        }


        public string FormatValue(string value)
        {
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
            SortedDictionary<int, List<string>> cmpDictionary = new SortedDictionary<int, List<string>>();

            // 0. Get input data
            FMG fileA = new FMG { Entries = FMG.Read(InputFiles[0]).Entries };
            FMG fileB = new FMG { Entries = FMG.Read(InputFiles[1]).Entries };

            // 1. Insert values from FileA
            int count = 0;
            foreach (FMG.Entry entry in fileA.Entries)
            {
                if (entry.Text == null) continue; // Exclude lines without value

                if (cmpDictionary.ContainsKey(entry.ID))
                {
                    Errors.Add( // The error is on file A (the mod file) as it is supposed to be the only one that could be incorrect
                        $"Unicity constraint error. Skipped entry ID {entry.ID} from input file A as it has already been registered.\r\n" +
                        $"  Entry Text =\r\n" +
                        $"\"{entry.Text}\""
                    );
                    continue;
                }
                count += 1;
                entry.Text = FormatValue(entry.Text);
                cmpDictionary.Add(entry.ID, new List<string>() { entry.Text, "" });

                if (preview && count == 50) break;
            }

            // 2. Insert values from FileB
            foreach (FMG.Entry entry in fileB.Entries)
            {
                if (preview && entry.ID > cmpDictionary.Keys.Max()) break; // Don't go higher than FileA IDs

                if (entry.Text == null) continue; // Exclude lines without value

                entry.Text = FormatValue(entry.Text);
                if (cmpDictionary.ContainsKey(entry.ID))
                {
                    cmpDictionary[entry.ID][1] = entry.Text;
                }
                else
                {
                    cmpDictionary.Add(entry.ID, new List<string>() { "", entry.Text });
                }
            }

            // 3. Compare values and build Entry
            foreach (KeyValuePair<int, List<string>> cmp in cmpDictionary)
            {
                Entries.Add(new CompareEntry(
                    cmp.Key,
                    cmp.Value[0],
                    cmp.Value[1],
                    (cmp.Value[0] == cmp.Value[1]).ToString()
                ));
            }
            if (preview)
            {
                Entries = Entries.Take(50).ToList(); // Enforce 50 as strict maximum
            }
        }


        public void ProduceOutput(string oFilename, string oHdr1, string oHdr2, string csvSepChar)
        {
            OutputFilename = Tools.GetOutputFilepath(oFilename);
            OutputHeaderA = oHdr1;
            OutputHeaderB = oHdr2;
            Sep = csvSepChar[0];

            using (StreamWriter writer = new StreamWriter(OutputFilename, false))
            {
                writer.WriteLine($"Text ID{Sep}{OutputHeaderA}{Sep}{OutputHeaderB}{Sep}Same?");
                foreach (CompareEntry ce in Entries)
                {
                    ce.ValueA = ce.ValueA.Replace("\"", "\"\"");
                    ce.ValueB = ce.ValueB.Replace("\"", "\"\"");

                    writer.WriteLine($"{ce.TextId}{Sep}\"{ce.ValueA}\"{Sep}\"{ce.ValueB}\"{Sep}{ce.Same}");
                }
            }
        }
    }


    public class CompareEntry
    {
        public int TextId { get; set; }
        public string ValueA { get; set; }
        public string ValueB { get; set; }
        public string Same { get; set; }

        public CompareEntry(int textId, string valueA, string valueB, string same)
        {
            TextId = textId;
            ValueA = valueA;
            ValueB = valueB;
            Same = same;
        }
    }

}
