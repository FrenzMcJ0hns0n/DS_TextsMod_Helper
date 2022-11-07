using System.Collections.Generic;
using System.Linq;

using SoulsFormats;

namespace DS_TextsMod_Helper
{
    public class PrepareMode
    {
        public string Title { get; set; }
        public List<string> InputFiles { get; set; }
        public string TextToReplace { get; set; }
        public string ReplacingText { get; set; }
        public string OutputFilename { get; set; }
        public FMG.FMGVersion OutputVersion { get; set; }
        public List<PrepareEntry> Entries { get; set; }


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


        public List<string> GetSpecialCases()
        {
            List<string> result = new List<string>();

            List<PrepareEntry> involvedEntries = Entries
                                         .Select(x => x)
                                         .Where(x => x.SpecialCase)
                                         .ToList();

            foreach (PrepareEntry e in involvedEntries)
                result.Add($"Entry Id {e.TextId} : Values of files A and B are identical and not empty, but value of file C was empty");

            return result;
        }


        public void ProcessFiles(bool preview)
        {
            Dictionary<int, List<string>> prpDictionary = new Dictionary<int, List<string>>();

            // 0. Get input data
            FMG fileA = new FMG { Entries = FMG.Read(InputFiles[0]).Entries };
            FMG fileB = new FMG { Entries = FMG.Read(InputFiles[1]).Entries };
            FMG fileC = new FMG { Entries = FMG.Read(InputFiles[2]).Entries };

            // 1. Take all values from FileA
            int counter = 0;
            foreach (FMG.Entry entry in fileA.Entries)
            {
                counter += 1;

                entry.Text = FormatValue(entry.Text);
                prpDictionary.Add(entry.ID, new List<string>() { entry.Text, "", "" });

                if (preview && counter == 50) // TODO? v1.6: Give choice about max results in Preview
                    break;
            }
            // 2. Insert entry.value from FileB if entry.ID in FileA
            foreach (FMG.Entry entry in fileB.Entries)
            {
                if (entry.Text == null)
                    continue;

                entry.Text = FormatValue(entry.Text);

                if (prpDictionary.ContainsKey(entry.ID))
                    prpDictionary[entry.ID][1] = entry.Text;
            }
            // 3. Insert entry.value from FileC if entry.ID in FileA
            foreach (FMG.Entry entry in fileC.Entries)
            {
                if (entry.Text == null)
                    continue;

                entry.Text = FormatValue(entry.Text);

                if (prpDictionary.ContainsKey(entry.ID))
                    prpDictionary[entry.ID][2] = entry.Text;
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
                    prp.Value[0] == prp.Value[1] ? "File C" : "File A",
                    (prp.Value[0] != "") && (prp.Value[0] == prp.Value[1]) && (prp.Value[2] == "") // (val1 != "") && (val1 == val2) && (val3 == "");
                ));
            }
        }


        public void ProduceOutput(string oFilename)
        {
            OutputFilename = Tools.GetOutputFilepath(oFilename);

            FMG output = new FMG { Version = OutputVersion };
            foreach (PrepareEntry pe in Entries)
                output.Entries.Add(new FMG.Entry(pe.TextId, pe.Output));

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
        public bool SpecialCase { get; set; }

        public PrepareEntry(int textId, string valueA, string valueB, string valueC, string output, string source, bool specialCase)
        {
            TextId = textId;
            ValueA = valueA;
            ValueB = valueB;
            ValueC = valueC;
            Output = output;
            Source = source;
            SpecialCase = specialCase;
        }
    }

}
