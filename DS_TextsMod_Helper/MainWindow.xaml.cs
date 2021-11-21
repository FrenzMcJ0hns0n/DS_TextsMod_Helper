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


            tbx_file1.Text = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED\pre-doa-backup\msg\ENGLISH\item.msgbnd.extract\FRPG\data\Msg\Data_ENGLISH\Accessory_long_desc_.fmg.csv";
            tbx_file2.Text = @"C:\Program Files (x86)\Steam\steamapps\common\DARK SOULS REMASTERED\msg\ENGLISH\item.msgbnd.extract\FRPG\data\Msg\Data_ENGLISH\RingDescriptions.fmg.csv";
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
                SortedDictionary<int, string> sdict = ReturnCompareDictionary(tbx_file1.Text, tbx_file2.Text, tbx_csvsepi.Text[0], tbx_csvsepo.Text[0], true);
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
                Dictionary<int, string> dict = ReturnPrepareDictionary(tbx_file1.Text, tbx_file2.Text, tbx_file3.Text, tbx_csvsepi.Text[0], tbx_csvsepo.Text[0], true);
                DisplayPreviewPrepare(dict, tbx_csvsepo.Text[0]);

                btn_execute.IsEnabled = true;
            }
        }




        private void btn_execute_Click(object sender, RoutedEventArgs e)
        {
            string output_filename = tbx_outputfilename.Text + ".csv";

            if (compare_clicked)
            {
                SortedDictionary<int, string> sdict = ReturnCompareDictionary(tbx_file1.Text, tbx_file2.Text, tbx_csvsepi.Text[0], tbx_csvsepo.Text[0], false);
                DoCompare(sdict, tbx_csvsepo.Text[0], tbx_header1.Text, tbx_header2.Text, output_filename);
                MessageBox.Show($"Compare mode: File \"{output_filename}\" created.");
            }

            if (prepare_clicked)
            {
                Dictionary<int, string> dict = ReturnPrepareDictionary(tbx_file1.Text, tbx_file2.Text, tbx_file3.Text, tbx_csvsepi.Text[0], tbx_csvsepo.Text[0], false);
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
            if (file_info.Extension.ToLowerInvariant() != ".csv")
                return false;

            string[] lines = File.ReadAllLines(file_info.FullName);
            if (lines.Length < 3)
                return false;

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
                MessageBox.Show("Error : empty output headers");
                return false;
            }

            if (output_filename == "")
            {
                MessageBox.Show("Error : output filename not specified");
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
            string output_preview = $"Text ID{osep}{oheader1}{osep}{oheader2}{osep}Same?"; // => "Text ID|Header 1|Header 2|Same?"
            foreach (KeyValuePair<int, string> od in dictionary_compare)
            {
                output_preview += $"\n{od.Value}";
            }

            tbk_preview.Text = output_preview;
        }

        private void DisplayPreviewPrepare(Dictionary<int, string> dictionary_prepare, char osep)
        {
            string output_preview = "";
            foreach (KeyValuePair<int, string> od in dictionary_prepare)
            {
                output_preview += (output_preview == "") ? $"{od.Key}{osep}{od.Value}" : $"\n{od.Key}{osep}{od.Value}"; // => "Text ID|Value"
            }

            tbk_preview.Text = output_preview;
        }




        private void DoCompare(SortedDictionary<int, string> dictionary_compare, char osep, string oheader1, string oheader2, string ofilename)
        {
            string output_filepath = Path.Combine(ReturnOutputDirectoryPath(), ofilename);
            using (StreamWriter writer = new StreamWriter(output_filepath, false))
            {
                writer.WriteLine($"Text ID{osep}{oheader1}{osep}{oheader2}{osep}Same?"); // => "Text ID|Header 1|Header 2|Same?"
                foreach (KeyValuePair<int, string> od in dictionary_compare)
                {
                    writer.WriteLine(od.Value);
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
                    writer.WriteLine($"{od.Key}{osep}{od.Value}"); // => "Text ID|Value"
                }
            }
        }



        private string FormatValue(string input)
        {
            string output = input.Trim();
            
            string trim_start = tbx_trimbegin.Text;
            int s = trim_start.Length;

            string trim_end = tbx_trimend.Text;
            int e = trim_end.Length;

            // Trim value as requested
            if (output.Substring(0, s) == trim_start)
                output = output.Substring(s);

            if (output.Substring(output.Length - e) == trim_end)
                output = output.Substring(0, output.Length - e);

            return output;
        }


        // --------------
        // Core functions
        // --------------
        private SortedDictionary<int,string> ReturnCompareDictionary(string ifile1, string ifile2, char isep, char osep, bool preview)
        {
            // Get input data from both File1 & File2
            string[] file_1_lines = File.ReadAllLines(ifile1);
            string[] file_2_lines = File.ReadAllLines(ifile2);

            int line_counter = 0;

            SortedDictionary<int, string> cmp_dictionary = new SortedDictionary<int, string>();

            // First, fill the dictionary with data from File1
            foreach (string line in file_1_lines)
            {
                string str_id = line.Split(isep)[0].Trim();
                string value1 = line.Split(isep)[1]; // Do not trim yet

                // Exclude lines without value
                if (value1 == "")
                    continue;

                // Do not exclude value1 " "
                if (value1 != " ")
                    value1 = FormatValue(value1);

                if (int.TryParse(str_id, out int id))
                {
                    cmp_dictionary.Add(id, value1);
                    line_counter += 1;
                }

                if (preview && line_counter == 6)
                    break;
            }


            // Then, compare data of File1 & File2
            line_counter = 0;
            foreach (string line in file_2_lines)
            {
                string str_id = line.Split(isep)[0].Trim();
                string value2 = line.Split(isep)[1]; // Do not trim yet

                // Exclude lines without value
                if (value2 == "")
                    continue;

                // Do not exclude value2 " "
                if (value2 != " ")
                    value2 = FormatValue(value2);

                if (int.TryParse(str_id, out int id))
                {
                    if (cmp_dictionary.ContainsKey(id)) // The key (id) has a value in both File1 & File2
                    {
                        if (cmp_dictionary.TryGetValue(id, out string value1))
                            cmp_dictionary[id] = $"{id}{osep}{value1}{osep}{value2}{osep}{value1 == value2}"; // => "id|value1|value2|true/false"
                    }
                    else // The key (id) has a value only in File2
                        cmp_dictionary.Add(id, $"{id}{osep}{osep}{value2}{osep}false"); // => "id||value2|false"

                    line_counter += 1;
                }

                if (preview && line_counter == 6)
                    break;
            }


            // Finally, get back on formatting values from File1
            SortedDictionary<int, string> replica_dictionary = new SortedDictionary<int, string>(cmp_dictionary);
            foreach (KeyValuePair<int, string> rd in replica_dictionary)
            {
                if (!rd.Value.Contains($"{osep}true") && !rd.Value.Contains($"{osep}false"))
                {
                    if (replica_dictionary.TryGetValue(rd.Key, out string value1)) // The key (rd.Key) has a value only in File1
                    {
                        cmp_dictionary[rd.Key] = $"{rd.Key}{osep}{value1}{osep}{osep}false"; // => "id|value1||false"
                    }
                }
            }

            return cmp_dictionary;
        }




        private Dictionary<int, string> ReturnPrepareDictionary(string ifile1, string ifile2, string ifile3, char isep, char osep, bool preview)
        {
            // Get input data from all files
            string[] file_1_lines = File.ReadAllLines(ifile1);
            string[] file_2_lines = File.ReadAllLines(ifile2);
            string[] file_3_lines = File.ReadAllLines(ifile3);

            int line_counter = 0;

            Dictionary<int, string> prp_dictionary = new Dictionary<int, string>();

            // First, fill the dictionary with ALL data from File1
            foreach (string line in file_1_lines)
            {
                string str_id = line.Split(isep)[0].Trim();
                string value1 = line.Split(isep)[1]; // Do not trim yet

                // Do not exclude value1 " "
                if (value1 != " ")
                    value1 = FormatValue(value1);

                if (int.TryParse(str_id, out int id))
                {
                    line_counter += 1;
                    prp_dictionary.Add(id, value1); // Contains File1 value so far
                }

                if (preview && line_counter == 7)
                    break;
            }

            // Then, update values of dictionary when File1 & File2 values are identical
            foreach (string line in file_2_lines)
            {
                string str_id = line.Split(isep)[0].Trim();
                string value2 = line.Split(isep)[1]; // Do not trim yet

                // Preserve value2 " "
                if (value2 != " ")
                    value2 = FormatValue(value2);

                if (int.TryParse(str_id, out int id))
                {
                    if (prp_dictionary.ContainsKey(id))
                    {
                        if (prp_dictionary.TryGetValue(id, out string value1))
                        {
                            if (value1 == value2)
                                prp_dictionary[id] = ""; // Erase, to be replaced with File3 value
                        }
                    }
                    // else : File2 value ignored as not in File1 structure
                }
            }

            // Finally, replace empty values with File3 ones
            foreach (string line in file_3_lines)
            {
                string str_id = line.Split(isep)[0].Trim();
                string value3 = line.Split(isep)[1]; // Do not trim yet

                // Preserve value3 " "
                if (value3 != " ")
                    value3 = FormatValue(value3);

                if (int.TryParse(str_id, out int id))
                {
                    if (prp_dictionary.ContainsKey(id))
                    {
                        if (prp_dictionary.TryGetValue(id, out string value1))
                        {
                            if (value1 == "")
                                prp_dictionary[id] = value3; // Insert File3 value
                        }
                    }
                    // else : File3 value ignored as not in File1 structure
                }
            }

            return prp_dictionary;
        }

    }
}
