﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DS_TextsMod_Helper
{
    internal class CompareMode : IProcessingModes
    {
        public List<string> InputFiles { get; set; }
        public string OutputFilename { get; set; }
        public string OutputHeader1 { get; set; }
        public string OutputHeader2 { get; set; }
        public char CsvSeparator { get; set; }
        public bool OneLinedValues { get; set; }
        public List<Entry> Entries { get; set; }


        public CompareMode(string iFile1, string iFile2) //, string oFilename, string oHdr1, string oHdr2, char oCsvSep
        {
            InputFiles = new List<string>() { iFile1, iFile2 };
            //OutputFilename = oFilename;
            //OutputHeader1 = oHdr1;
            //OutputHeader2 = oHdr2;
            //CsvSeparator = oCsvSep;
            //Entries = new List<Entry>();
        }


        public void CheckFiles()
        {
            foreach (string filepath in InputFiles)
            {
                try
                {
                    FileInfo info = new FileInfo(filepath);

                    if (info.Extension != ".fmg")
                    {
                        _ = MessageBox.Show("Error: File extension must be .fmg");
                        return;
                    }

                    _ = SoulsFormats.FMG.Read(filepath);
                }
                catch (Exception ex)
                {
                    _ = MessageBox.Show($"Error in method CompareMode.CheckFiles.\nException: {ex}");
                }

            }
        }


        public string FormatValue(string value)
        {
            //TODO: Check several spaces?
            if (value == " ")
                return value;

            if (OneLinedValues)
                return value.Replace("\n", "/n/").Trim();

            return value.Trim();
        }


        public void ProcessFiles(bool preview)
        {
            Entries = new List<Entry>();

            // Get input data
            SoulsFormats.FMG file_1 = new SoulsFormats.FMG { Entries = SoulsFormats.FMG.Read(InputFiles[0]).Entries };
            SoulsFormats.FMG file_2 = new SoulsFormats.FMG { Entries = SoulsFormats.FMG.Read(InputFiles[1]).Entries };

            SortedDictionary<int, List<string>> cmp_dictionary = new SortedDictionary<int, List<string>>();

            // 1. Fill dictionary with data from File1
            int counter = 0;
            foreach (SoulsFormats.FMG.Entry entry in file_1.Entries)
            {
                if (entry.Text == null) // Exclude lines without value
                    continue;

                entry.Text = FormatValue(entry.Text);
                cmp_dictionary.Add(entry.ID, new List<string>() { entry.Text, "" }); // cmp_dictionary[TextId] = { File1_value, "" }
                
                counter += 1;
                if (preview && counter == 30)
                    break;
            }

            // 2. Fill dictionary with data from File2
            counter = 0;
            foreach (SoulsFormats.FMG.Entry entry in file_2.Entries)
            {
                if (entry.Text == null) // Exclude lines without value
                    continue;

                entry.Text = FormatValue(entry.Text);
                if (cmp_dictionary.ContainsKey(entry.ID))
                    cmp_dictionary[entry.ID][1] = entry.Text; // cmp_dictionary[TextId] = { File1_value, File2_value }
                else
                    cmp_dictionary.Add(entry.ID, new List<string>() { "", entry.Text }); // cmp_dictionary[TextId] = { "", File2_value }

                counter += 1;
                if (preview && counter == 30)
                    break;
            }

            // 3. Compare values and build final Objects
            int index = 0;
            foreach (KeyValuePair<int, List<string>> cmp in cmp_dictionary)
            {
                index += 1;
                int textId = cmp.Key;
                string val1 = cmp.Value[0];
                string val2 = cmp.Value[1];
                bool isSame = cmp.Value[0] == cmp.Value[1];

                Entries.Add(new Entry(index, textId, val1, val2, isSame.ToString()));
            }
        }


        public class Entry
        {
            public int Index { get; set; }
            public int TextId { get; set; }
            public string Value1 { get; set; }
            public string Value2 { get; set; }
            public string Same { get; set; }

            public Entry(int index, int textId, string value1, string value2, string same)
            {
                Index = index;
                TextId = textId;
                Value1 = value1;
                Value2 = value2;
                Same = same;
            }
        }

    }
}