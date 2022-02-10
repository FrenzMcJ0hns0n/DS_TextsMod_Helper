using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DS_TextsMod_Helper
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //TODO: [General] Reorganize code in several files/classes




        // Hacky shortcuts // TODO: Improve
        private static bool compare_clicked = false;
        private static bool prepare_clicked = false;




        public MainWindow()
        {
            InitializeComponent();

            Directory.CreateDirectory(ReturnOutputDirectoryPath());
            FindSoulsFormatsDll();
        }




        private string ReturnRootDirectoryPath()
        {
            return Directory.GetParent(Assembly.GetExecutingAssembly().Location).ToString();
        }

        private string ReturnOutputDirectoryPath()
        {
            return Path.Combine(ReturnRootDirectoryPath(), "Output");
        }

        private void FindSoulsFormatsDll()
        {
            if (!File.Exists(Path.Combine(ReturnRootDirectoryPath(), "SoulsFormats.dll")))
                MessageBox.Show("Fatal error : file 'SoulsFormats.dll' not found");
        }




        // ----------
        // GUI events
        // ----------
        private void tbx_file1_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
        private void tbx_file1_Drop(object sender, DragEventArgs e)
        {
            if (AcceptDroppedInputFile(e))
            {
                DisplayInputFilepath(sender, e);
                SyncFilename();
            }
                
        }

        private void tbx_file2_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
        private void tbx_file2_Drop(object sender, DragEventArgs e)
        {
            if (AcceptDroppedInputFile(e))
                DisplayInputFilepath(sender, e);
        }

        private void tbx_file3_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
        private void tbx_file3_Drop(object sender, DragEventArgs e)
        {
            if (AcceptDroppedInputFile(e))
                DisplayInputFilepath(sender, e);
        }

        private void cbx_syncoutputfilename_Checked(object sender, RoutedEventArgs e)
        {
            SyncFilename();
        }




        private void btn_comparefiles_Click(object sender, RoutedEventArgs e)
        {
            // TODO: "Disable" useless controls for this mode : tbx_file3

            btn_execute.IsEnabled = false;

            compare_clicked = true;
            prepare_clicked = false;

            lbl_outputfileext.Content = ".csv";
            tbk_summary.Text = "[Compare/Compile] To generate a new CSV file with data comparison\n"
                             + "Expected: File #1 is the modded file in {mod author's language}. File #2 is the same file in vanilla, in same language.\n"
                             + "The program will loop through contents of both files, scanning their entries:\n"
                             + "    For a given entry, do the files have a value and is it the same?\n"
                             + "        - None of them has a value skip: skip the entry = Write nothing in output file\n"
                             + "        - At least one of them has a value: compare both = Write \"True\" if identical or \"False\" if not";

            if (CompareModeReady())
            {
                SortedDictionary<int, string> sdict = ReturnCompareDictionary(tbx_file1.Text, tbx_file2.Text, tbx_csvsepo.Text[0], true);
                DisplayPreviewCompare(sdict, tbx_csvsepo.Text[0], tbx_header1.Text, tbx_header2.Text);

                btn_execute.IsEnabled = true;
            }
        }

        private void btn_preparefile_Click(object sender, RoutedEventArgs e)
        {
            // TODO: "Disable" useless controls for this mode : tbx_header1, tbx_header2

            btn_execute.IsEnabled = false;

            compare_clicked = false;
            prepare_clicked = true;

            lbl_outputfileext.Content = ".fmg";
            tbk_summary.Text = "[Prepare/Comply] To prepare a new FMG file for mod translation\n"
                             + "Expected: File #1 is the modded file in {mod author's language}. File #2 is the same file in vanilla, in same language. "
                             + "File #3 is the same file in vanilla, in {target new language}.\n"
                             + "The program will replicate structure of file #1, and fill it with values dynamically, depending on comparison between #1 and #2:\n"
                             + "    For each File #1 entry:\n"
                             + "        - Files #1 and #2 has identical values: use value from file #3, as this is the vanilla content and translation already exists\n"
                             + "        - Files #1 and #2 has different values: use value from file #1, and let the translator update it manually";

            if (PrepareModeReady())
            {
                Dictionary<int, string> dict = ReturnPrepareDictionary(tbx_file1.Text, tbx_file2.Text, tbx_file3.Text, true);
                DisplayPreviewPrepare(dict, tbx_csvsepo.Text[0]);

                btn_execute.IsEnabled = true;
            }
        }




        private void btn_execute_Click(object sender, RoutedEventArgs e)
        {
            string output_filename = tbx_outputfilename.Text + lbl_outputfileext.Content;

            if (compare_clicked)
            {
                SortedDictionary<int, string> sdict = ReturnCompareDictionary(tbx_file1.Text, tbx_file2.Text, tbx_csvsepo.Text[0], false);
                DoCompare(sdict, tbx_csvsepo.Text[0], tbx_header1.Text, tbx_header2.Text, output_filename);
                MessageBox.Show($"Compare mode: File \"{output_filename}\" created.");
            }

            if (prepare_clicked)
            {
                Dictionary<int, string> dict = ReturnPrepareDictionary(tbx_file1.Text, tbx_file2.Text, tbx_file3.Text, false);
                DoPrepare(dict, tbx_csvsepo.Text[0], output_filename);
                MessageBox.Show($"Prepare mode: File \"{output_filename}\" created.");
            }
        }


        // --------------------
        // Validation & display
        // --------------------
        private bool AcceptDroppedInputFile(DragEventArgs e)
        {
            FileInfo file_info = new FileInfo(((string[])e.Data.GetData(DataFormats.FileDrop))[0]);
            if (file_info.Extension.ToLowerInvariant() != ".fmg")
                return false;

            try { SoulsFormats.FMG.Read(file_info.FullName); }
            catch (Exception ex)
            {
                MessageBox.Show("Error while reading this input file : '" + ex.ToString() + "'");
                return false;
            }
            return true;
        }

        private void DisplayInputFilepath(object sender, DragEventArgs e)
        {
            string filepath = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            TextBox tbx = sender as TextBox;

            tbx.Text = filepath;
            tbx.FontStyle = FontStyles.Normal;
            tbx.Foreground = Brushes.Black;
        }

        private void SyncFilename()
        {
            if (cbx_syncoutputfilename.IsChecked == false)
                return;

            if (tbx_file1.Text == "Drop FMG file...")
                return;

            FileInfo file_info = new FileInfo(tbx_file1.Text);
            tbx_outputfilename.Text = file_info.Name.Substring(0, file_info.Name.Length - file_info.Extension.Length);
        }

        private bool CompareModeReady()
        {
            string input_filepath1 = tbx_file1.Text;
            string input_filepath2 = tbx_file2.Text;
            string output_header_1 = tbx_header1.Text;
            string output_header_2 = tbx_header2.Text;
            string output_filename = tbx_outputfilename.Text;
            string sepa_csv_char_o = tbx_csvsepo.Text;

            if (!File.Exists(input_filepath1) || !File.Exists(input_filepath2))
            {
                MessageBox.Show("Error : missing input file(s)");
                return false;
            }

            if (input_filepath1 == input_filepath2)
            {
                MessageBox.Show("Error : same file submitted twice");
                return false;
            }

            if (output_header_1 == "" || output_header_2 == "")
            {
                MessageBox.Show("Error : missing output header(s)");
                return false;
            }

            if (output_filename == "")
            {
                MessageBox.Show("Error : output filename not specified");
                return false;
            }

            if (sepa_csv_char_o == "")
            {
                MessageBox.Show("Error : missing CSV separator");
                return false;
            }

            return true;
        }

        private bool PrepareModeReady()
        {
            string input_filepath1 = tbx_file1.Text;
            string input_filepath2 = tbx_file2.Text;
            string input_filepath3 = tbx_file3.Text;
            string output_filename = tbx_outputfilename.Text;

            if (!File.Exists(input_filepath1) || !File.Exists(input_filepath2) || !File.Exists(input_filepath3))
            {
                MessageBox.Show("Error : missing input file(s)");
                return false;
            }

            if ((input_filepath1 == input_filepath2) || (input_filepath2 == input_filepath3) || (input_filepath1 == input_filepath3))
            {
                MessageBox.Show("Error : same file submitted several times");
                return false;
            }

            if (output_filename == "")
            {
                MessageBox.Show("Error : output filename not specified");
                return false;
            }

            return true;
        }




        // ----------------------
        // Calling core functions
        // ----------------------
        private void DisplayPreviewCompare(SortedDictionary<int, string> dictionary_compare, char osep, string oheader1, string oheader2)
        {
            string output_preview = $"Text ID{osep}{oheader1}{osep}{oheader2}{osep}Same?"; // Headers = "Text ID|Header 1|Header 2|Same?"
            foreach (KeyValuePair<int, string> od in dictionary_compare)
            {
                output_preview += $"\n{od.Value}"; // Data = "Text ID|Value 1|Value 2|[True/False]"
            }

            tbk_preview.Text = output_preview;
        }

        private void DisplayPreviewPrepare(Dictionary<int, string> dictionary_prepare, char osep)
        {
            string output_preview = ""; // Headers = none
            foreach (KeyValuePair<int, string> od in dictionary_prepare)
            {
                output_preview += (output_preview == "") ? $"{od.Key}{osep}{od.Value}" : $"\n{od.Key}{osep}{od.Value}"; // Data = "Text ID|Value"
            }

            tbk_preview.Text = output_preview;
        }




        private void DoCompare(SortedDictionary<int, string> dictionary_compare, char osep, string oheader1, string oheader2, string ofilename)
        {
            string output_filepath = Path.Combine(ReturnOutputDirectoryPath(), ofilename);
            using (StreamWriter writer = new StreamWriter(output_filepath, false))
            {
                writer.WriteLine($"Text ID{osep}{oheader1}{osep}{oheader2}{osep}Same?"); // Headers = "Text ID|Header 1|Header 2|Same?"
                foreach (KeyValuePair<int, string> od in dictionary_compare)
                {
                    writer.WriteLine(od.Value); // Data = "Text ID|Value 1|Value 2|[True/False]"
                }
            }
        }

        private void DoPrepare(Dictionary<int, string> dictionary_prepare, char osep, string ofilename)
        {
            string output_filepath = Path.Combine(ReturnOutputDirectoryPath(), ofilename);
            SoulsFormats.FMG output = new SoulsFormats.FMG { };
            foreach (KeyValuePair<int, string> od in dictionary_prepare)
            {
                output.Entries.Add(new SoulsFormats.FMG.Entry(od.Key, (od.Value == "") ? null : od.Value)); // Data = "Text ID|Value"
            }
            output.Write(output_filepath);
    }




        // -----------------
        // String operations
        // -----------------
        private string FormatValue(string value)
        {
            return ReplaceValue(value);
        }
        private string ReplaceValue(string txt)
        {
            if (tbx_replacesource.Text == "")
                return txt;

            if (txt.Contains(tbx_replacesource.Text))
                return txt.Replace(tbx_replacesource.Text, tbx_replacedest.Text);

            if (tbx_replacesource.Text == "\\n") // This seems hacky to me... TODO? Improve
                return txt.Replace("\n", tbx_replacedest.Text);

            return txt;
        }


        // --------------
        // Core functions
        // --------------
        private SortedDictionary<int,string> ReturnCompareDictionary(string ifile1, string ifile2, char osep, bool preview)
        {
            // Get input data
            SoulsFormats.FMG file_1 = new SoulsFormats.FMG { Entries = SoulsFormats.FMG.Read(ifile1).Entries };
            SoulsFormats.FMG file_2 = new SoulsFormats.FMG { Entries = SoulsFormats.FMG.Read(ifile2).Entries };

            SortedDictionary<int, string> cmp_dictionary = new SortedDictionary<int, string>();

            // 1. Fill the dictionary with data from File1
            int counter = 0;
            foreach (SoulsFormats.FMG.Entry entry in file_1.Entries)
            {
                if (entry.Text == null) // Exclude lines without value
                    continue;

                if (entry.Text != " ") // Preserve Text = " "
                    entry.Text = FormatValue(entry.Text);

                cmp_dictionary.Add(entry.ID, entry.Text);
                counter += 1;

                if (preview && counter == 6)
                    break;
            }

            // 2. Compare data of File1 & File2
            counter = 0;
            foreach(SoulsFormats.FMG.Entry entry in file_2.Entries)
            {
                if (entry.Text == null) // Exclude lines without value
                    continue;

                if (entry.Text != " ") // Preserve Text = " "
                    entry.Text = FormatValue(entry.Text);

                if (cmp_dictionary.ContainsKey(entry.ID)) // The key (id) has a value in both File1 & File2
                {
                    if (cmp_dictionary.TryGetValue(entry.ID, out string file1_value))
                        cmp_dictionary[entry.ID] = $"{entry.ID}{osep}{file1_value}{osep}{entry.Text}{osep}{(file1_value == entry.Text ? "true" : "false")}"; // => "id|value1|value2|true/false"
                }
                else // The key (id) has a value only in File2
                    cmp_dictionary.Add(entry.ID, $"{entry.ID}{osep}{osep}{entry.Text}{osep}false"); // => "id||value2|false"

                counter += 1;

                if (preview && counter == 6)
                    break;
            }

            // 3. Get back on formatting values from File1
            SortedDictionary<int, string> replica_dictionary = new SortedDictionary<int, string>(cmp_dictionary);
            foreach (KeyValuePair<int, string> rd in replica_dictionary)
            {
                if (!rd.Value.Contains($"{osep}true") && !rd.Value.Contains($"{osep}false"))
                {
                    if (replica_dictionary.TryGetValue(rd.Key, out string value1)) // The key (rd.Key) has a value only in File1
                        cmp_dictionary[rd.Key] = $"{rd.Key}{osep}{value1}{osep}{osep}false"; // => "id|value1||false"
                }
            }

            return cmp_dictionary;
        }




        private Dictionary<int, string> ReturnPrepareDictionary(string ifile1, string ifile2, string ifile3, bool preview)
        {
            // Get input data
            SoulsFormats.FMG file_1 = new SoulsFormats.FMG { Entries = SoulsFormats.FMG.Read(ifile1).Entries };
            SoulsFormats.FMG file_2 = new SoulsFormats.FMG { Entries = SoulsFormats.FMG.Read(ifile2).Entries };
            SoulsFormats.FMG file_3 = new SoulsFormats.FMG { Entries = SoulsFormats.FMG.Read(ifile3).Entries };

            Dictionary<int, string> prp_dictionary = new Dictionary<int, string>();

            // 1. Fill the dictionary with ALL data from File1
            int counter = 0;
            foreach (SoulsFormats.FMG.Entry entry in file_1.Entries)
            {
                if (entry.Text == null)
                    prp_dictionary.Add(entry.ID, "");

                if (entry.Text != " ") // Preserve Text = " "
                    entry.Text = FormatValue(entry.Text);

                counter += 1;
                prp_dictionary.Add(entry.ID, entry.Text); // At this point, the dictionary contains File1 values 

                if (preview && counter == 7)
                    break;
            }

            // 2. Update values of dictionary when File1 & File2 values are identical
            foreach (SoulsFormats.FMG.Entry entry in file_2.Entries)
            {
                if (entry.Text == null)
                    continue;

                if (entry.Text != " ") // Preserve Text = " "
                    entry.Text = FormatValue(entry.Text);

                if (prp_dictionary.ContainsKey(entry.ID))
                {
                    if (prp_dictionary.TryGetValue(entry.ID, out string file1_value))
                    {
                        if (entry.Text == file1_value)
                            prp_dictionary[entry.ID] = "TO_BE_REPLACED"; // Will write File3 value
                    }
                } // else : File2 value ignored as not in File1 structure
            }

            // 3. Replace empty values with File3 ones
            foreach (SoulsFormats.FMG.Entry entry in file_3.Entries)
            {
                if (entry.Text == null)
                    continue;

                // Preserve Text = " "
                if (entry.Text != " ")
                    entry.Text = FormatValue(entry.Text);

                if (prp_dictionary.ContainsKey(entry.ID))
                {
                    if (prp_dictionary.TryGetValue(entry.ID, out string file1_value))
                    {
                        if (file1_value == "TO_BE_REPLACED")
                            prp_dictionary[entry.ID] = entry.Text; // Insert File3 value
                    }
                } // else : File3 value ignored as not in File1 structure
            }

            return prp_dictionary;
        }

    }
}
