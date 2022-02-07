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


            tbx_file1.Text = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED\pre-doa-backup\msg\ENGLISH\item.msgbnd.extract\FRPG\data\Msg\Data_ENGLISH\Item_long_desc_.fmg - test.csv";
            tbx_file2.Text = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED\msg\ENGLISH\item.msgbnd.extract\FRPG\data\Msg\Data_ENGLISH\ItemDescriptions.fmg - test.csv";
        }




        private string ReturnOutputDirectoryPath()
        {
            string src_executable = Assembly.GetExecutingAssembly().Location;
            string root_directory = Directory.GetParent(src_executable).ToString();
            return Path.Combine(root_directory, "Output");
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
                DisplayInputFilepath(sender, e);
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




        private void btn_comparefiles_Click(object sender, RoutedEventArgs e)
        {
            // TODO: "Disable" useless controls for this mode : tbx_file3

            btn_execute.IsEnabled = false;

            compare_clicked = true;
            prepare_clicked = false;

            tbk_summary.Text = "[Compare/Compile] To generate a new file with data comparison\n"
                             + "Expected: File #1 is the modded file in {mod author's language}. File #2 is the same file in vanilla, in same language.\n"
                             + "The program will loop through contents of both files, scanning their Text IDs and values:\n"
                             + "    For a given Text ID: do the files have a value and is it the same for both?\n"
                             + "        None of them has a value: skip the TextID (= line removed in output file)\n"
                             + "        At least one of them has a value: compare the values and give the information \"Are both values the same?\"\n";

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

            tbk_summary.Text = "[Prepare/Comply] To prepare a new file for mod translation\n"
                             + "Expected: File #1 is the modded file in {mod author's language}. File #2 is the same file in vanilla, in same language. "
                             + "File #3 is the same file in vanilla, in {target new language}.\n"
                             + "The program will replicate structure of file #1 regarding to its Text IDs, and compare data from #1 and #2 regarding to their values:\n"
                             + "    For a given Text ID, are their values the same?\n"
                             + "        Yes, files #1 and #2 has the same value: use value from file #3, as this is the vanilla content and translation already exists\n"
                             + "        No, files #1 and #2 has different values: use value from file #1, and let the translator update it manually";

            if (PrepareModeReady())
            {
                Dictionary<int, string> dict = ReturnPrepareDictionary(tbx_file1.Text, tbx_file2.Text, tbx_file3.Text, true);
                DisplayPreviewPrepare(dict, tbx_csvsepo.Text[0]);

                btn_execute.IsEnabled = true;
            }
        }




        private void btn_execute_Click(object sender, RoutedEventArgs e)
        {
            string output_filename = tbx_outputfilename.Text + ".csv";

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

            try { _ = SoulsFormats.FMG.Read(file_info.FullName); }
            catch { return false; }
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
            using (StreamWriter writer = new StreamWriter(output_filepath, false))
            {
                foreach (KeyValuePair<int, string> od in dictionary_prepare)
                {
                    writer.WriteLine($"{od.Key}{osep}{od.Value}"); // Data = "Text ID|Value"
                }
            }
        }




        // -----------------
        // String operations
        // -----------------
        private string FormatValue(string value)
        {
            value = TrimStart(value.Trim());
            value = TrimEnd(value.Trim());

            return value;
        }
        private string TrimStart(string txt)
        {
            string trim_start = tbx_trimbegin.Text;

            if (trim_start != "" && txt.Length > trim_start.Length)
            {
                while (txt.Substring(0, trim_start.Length) == trim_start)
                    txt = txt.Substring(trim_start.Length);
            }

            return txt;
        }
        private string TrimEnd(string txt)
        {
            string trim_end = tbx_trimend.Text;

            if (trim_end != "" && txt.Length > trim_end.Length)
            {
                while (txt.Substring(txt.Length - trim_end.Length) == trim_end)
                    txt = txt.Substring(0, txt.Length - trim_end.Length);
            }

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
                if (entry.Text == "") // Exclude lines without value
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
                if (entry.Text == "") // Exclude lines without value
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
                if (entry.Text != " ") // Preserve Text = " "
                    entry.Text = FormatValue(entry.Text);

                if (prp_dictionary.ContainsKey(entry.ID))
                {
                    if (prp_dictionary.TryGetValue(entry.ID, out string file1_value))
                    {
                        if (entry.Text == file1_value)
                            prp_dictionary[entry.ID] = ""; // Erase, to be replaced with File3 value
                    }
                } // else : File2 value ignored as not in File1 structure
            }

            // 3. Replace empty values with File3 ones
            foreach (SoulsFormats.FMG.Entry entry in file_3.Entries)
            {
                // Preserve Text = " "
                if (entry.Text != " ")
                    entry.Text = FormatValue(entry.Text);

                if (prp_dictionary.ContainsKey(entry.ID))
                {
                    if (prp_dictionary.TryGetValue(entry.ID, out string file1_value))
                    {
                        if (file1_value == "")
                            prp_dictionary[entry.ID] = entry.Text; // Insert File3 value
                    }
                } // else : File3 value ignored as not in File1 structure
            }

            return prp_dictionary;
        }

    }
}
