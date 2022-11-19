using System.Collections.Generic;
using System.Linq;

using SoulsFormats;

namespace DS_TextsMod_Helper
{
    public class PrepareMode
    {
        public List<PrepareEntry> Entries { get; set; }
        public List<string> Errors { get; set; }
        public List<string> InputFiles { get; set; }
        public string OutputFilename { get; set; }
        public FMG.FMGVersion OutputVersion { get; set; }
        public string ReplacingText { get; set; }
        public string TextToReplace { get; set; }
        public string Title { get; set; }



        public PrepareMode(string iFile1, string iFile2, string iFile3)
        {
            InputFiles = new List<string>() { iFile1, iFile2, iFile3 };
            Entries = new List<PrepareEntry>();
        }


        public string FormatValue(string value)
        {
            if (value is null)
                return value;

            // Preserve white spaces, whatever their purpose
            if (value.Count(char.IsWhiteSpace) == value.Length)
                return value;

            if (TextToReplace == "")
                return value.Trim();

            if (value.Contains(TextToReplace))
                return value.Replace(TextToReplace, ReplacingText).Trim();

            return value.Trim();
        }


        public void ProcessFiles(int maximum = 0) // "maximum" = entries max limit (default = unlimited)
        {
            Errors = new List<string>();
            Dictionary<int, List<string>> prpDictionary = new Dictionary<int, List<string>>();

            // 0. Get input data
            FMG fileA = new FMG { Entries = FMG.Read(InputFiles[0]).Entries };
            FMG fileB = new FMG { Entries = FMG.Read(InputFiles[1]).Entries };
            FMG fileC = new FMG { Entries = FMG.Read(InputFiles[2]).Entries };

            // 1. Take all values from FileA
            int count = 0;
            foreach (FMG.Entry entry in fileA.Entries)
            {
                if (prpDictionary.ContainsKey(entry.ID))
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
                prpDictionary.Add(entry.ID, new List<string>() { entry.Text, "", "" });

                if (count == maximum) break;
            }

            // 2. Insert entry.value from FileB if entry.ID in FileA
            foreach (FMG.Entry entry in fileB.Entries)
            {
                if (entry.Text == null) continue;

                entry.Text = FormatValue(entry.Text);
                if (prpDictionary.ContainsKey(entry.ID))
                {
                    prpDictionary[entry.ID][1] = entry.Text;
                }
            }

            // 3. Insert entry.value from FileC if entry.ID in FileA
            foreach (FMG.Entry entry in fileC.Entries)
            {
                if (entry.Text == null) continue;

                entry.Text = FormatValue(entry.Text);
                if (prpDictionary.ContainsKey(entry.ID))
                {
                    prpDictionary[entry.ID][2] = entry.Text;
                }
            }

            // 4. Compare values and build Entry
            foreach (KeyValuePair<int, List<string>> prp in prpDictionary)
            {
                Entries.Add(new PrepareEntry(
                    prp.Key,
                    prp.Value[0],
                    prp.Value[1],
                    prp.Value[2],
                    prp.Value[0] == prp.Value[1] ? prp.Value[2] : prp.Value[0],
                    prp.Value[0] == prp.Value[1] ? "File C" : "File A"
                ));

                // (valA != "") && (valA == valB) && (valC == "");
                if ((prp.Value[0] != "") && (prp.Value[0] == prp.Value[1]) && (prp.Value[2] == ""))
                {
                    Errors.Add(
                        $"Data warning. Removed value on entry ID {prp.Key} from File A in the following context :\r\n" +
                        "  Values from A & B are identical and not empty, but the reference value from C is empty.\r\n" +
                        "  This behavior is consistent with the logic of Prepare mode, but may cause a data loss.\r\n" +
                        "  File A value =\r\n" +
                        $"\"{prp.Value[0]}\""
                    );
                }
            }
        }


        public void ProduceOutput(string oFilename)
        {
            OutputFilename = Tools.GetOutputFilepath(oFilename);

            FMG output = new FMG { Version = OutputVersion };
            foreach (PrepareEntry pe in Entries)
            {
                output.Entries.Add(new FMG.Entry(pe.TextId, pe.Output));
            }
            output.Write(OutputFilename);
        }


        /// <summary>
        /// Translate back FMG version from String to FMG.FMGVersion
        /// </summary>
        public void SetOutputVersion(string version)
        {
            switch (version)
            {
                case "Demon's Souls":
                    OutputVersion = FMG.FMGVersion.DemonsSouls;
                    break;

                case "Dark Souls 3 / Bloodborne":
                    OutputVersion = FMG.FMGVersion.DarkSouls3;
                    break;

                default:
                    OutputVersion = FMG.FMGVersion.DarkSouls1;
                    break;
            }
        }
    }


    public class PrepareEntry
    {
        public int TextId { get; set; }
        public string ValueA { get; set; }
        public string ValueB { get; set; }
        public string ValueC { get; set; }
        public string Output { get; set; }
        public string Source { get; set; }

        public PrepareEntry(int textId, string valueA, string valueB, string valueC, string output, string source)
        {
            TextId = textId;
            ValueA = valueA;
            ValueB = valueB;
            ValueC = valueC;
            Output = output;
            Source = source;
        }
    }

}
