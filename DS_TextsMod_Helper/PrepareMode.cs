using System.Collections.Generic;
using System.Linq;

using SoulsFormats;

namespace DS_TextsMod_Helper
{
    public class PrepareMode : IProcessingModes
    {
        public List<string> InputFiles { get; set; }
        public string TextToReplace { get; set; }
        public string ReplacingText { get; set; }
        public string OutputFilename { get; set; }
        public FMG.FMGVersion OutputVersion { get; set; }
        public List<Entry> Entries { get; set; }


        public PrepareMode(string iFile1, string iFile2, string iFile3, string textToReplace, string replacingText)
        {
            InputFiles = new List<string>() { iFile1, iFile2, iFile3 };
            TextToReplace = textToReplace;
            ReplacingText = replacingText;
            Entries = new List<Entry>();
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

            if(value.Contains(TextToReplace))
                return value.Replace(TextToReplace, ReplacingText).Trim();

            return value.Trim();
        }

        public List<string> GetSpecialCases()
        {
            List<string> result = new List<string>();

            List<Entry> involvedEntries = Entries
                                         .Select(x => x)
                                         .Where(x => x.SpecialCase)
                                         .ToList();

            foreach (Entry e in involvedEntries)
                result.Add($"Entry Id {e.TextId} : Values of files A and B are identical and not empty, but value of file C was empty");

            return result;
        }

        public void ProcessFiles(bool preview)
        {
            // 0. Get input data
            FMG file_1 = new FMG { Entries = FMG.Read(InputFiles[0]).Entries };
            FMG file_2 = new FMG { Entries = FMG.Read(InputFiles[1]).Entries };
            FMG file_3 = new FMG { Entries = FMG.Read(InputFiles[2]).Entries };

            Dictionary<int, List<string>> prp_dictionary = new Dictionary<int, List<string>>();
            // 1. Take all values from File1
            int counter = 0;
            foreach (FMG.Entry entry in file_1.Entries)
            {
                entry.Text = FormatValue(entry.Text);
                prp_dictionary.Add(entry.ID, new List<string>() { entry.Text, "", "" });

                counter += 1;
                if (preview && counter == 50) // TODO? v1.4: Give choice about max results
                    break;
            }
            // 2. Insert value from File2 if entry.ID in File1
            foreach (FMG.Entry entry in file_2.Entries)
            {
                if (entry.Text == null)
                    continue;

                entry.Text = FormatValue(entry.Text);

                if (prp_dictionary.ContainsKey(entry.ID))
                    prp_dictionary[entry.ID][1] = entry.Text;
            }
            // 3. Insert value from File3 if entry.ID in File1
            foreach (FMG.Entry entry in file_3.Entries)
            {
                if (entry.Text == null)
                    continue;

                entry.Text = FormatValue(entry.Text);

                if (prp_dictionary.ContainsKey(entry.ID))
                    prp_dictionary[entry.ID][2] = entry.Text; 
            }

            // 4. Compare values and build Entry
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
                bool specialCase = val1 != "" && val1 == val2 && val3 == "";

                Entries.Add(new Entry(index, textId, val1, val2, val3, output, source, specialCase));
            }
        }

        public void ProduceOutput(string oFilename)
        {
            OutputFilename = Tools.GetOutputFilepath(oFilename);

            FMG output = new FMG { Version = OutputVersion };
            foreach (Entry pe in Entries)
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

                case "Dark Souls 1 / Dark Souls 2":
                    OutputVersion = FMG.FMGVersion.DarkSouls1;
                    break;

                case "Dark Souls 3 / Bloodborne":
                    OutputVersion = FMG.FMGVersion.DarkSouls3;
                    break;

                default:
                    OutputVersion = FMG.FMGVersion.DarkSouls1;
                    break;
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
            public bool SpecialCase { get; set; }

            public Entry(int index, int textId, string value1, string value2, string value3, string output, string source, bool specialCase)
            {
                Index = index;
                TextId = textId;
                Value1 = value1;
                Value2 = value2;
                Value3 = value3;
                Output = output;
                Source = source;
                SpecialCase = specialCase;
            }
        }

    }
}
