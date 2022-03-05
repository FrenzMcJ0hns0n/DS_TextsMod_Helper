using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DS_TextsMod_Helper
{
    /// <summary>
    /// Logique d'interaction pour MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {



        // Hacky shortcuts // TODO: Improve
        private static bool compare_clicked = false;
        private static bool prepare_clicked = false;




        public MainWindow()
        {
            InitializeComponent();

            Directory.CreateDirectory(ReturnOutputDirectoryPath());
            FindSoulsFormatsDll();

#if DEBUG
            tbx_file1_compare.Text = @"C:\Sandbox\Modding data\DS_TMH\published 1.2\FMG test files\English - Daughters of Ash\RingDescriptions.fmg";
            tbx_file2_compare.Text = @"C:\Sandbox\Modding data\DS_TMH\published 1.2\FMG test files\English - vanilla\Accessory_long_desc_.fmg";

            //tbx_file1_prepare.Text = @"C:\Sandbox\Modding data\DS_TMH\published 1.2\FMG test files\English - Daughters of Ash\RingDescriptions.fmg";
            //tbx_file2_prepare.Text = @"C:\Sandbox\Modding data\DS_TMH\published 1.2\FMG test files\English - vanilla\Accessory_long_desc_.fmg";
            //tbx_file3_prepare.Text = @"C:\Sandbox\Modding data\DS_TMH\published 1.2\FMG test files\French - vanilla\Accessory_long_desc_.fmg";

            //tbx_file1_compare.Text = @"C:\Sandbox\Modding data\DS_TMH\work in progress 1.3\FMG test files\Items - English - vanilla\Item_name_.fmg";
            //tbx_file2_compare.Text = @"C:\Sandbox\Modding data\DS_TMH\work in progress 1.3\FMG test files\Items - English - Daughters of Ash\ItemNames.fmg";

            tbx_file1_prepare.Text = @"C:\Sandbox\Modding data\DS_TMH\work in progress 1.3\FMG test files\Items - English - Daughters of Ash\ArmorDescriptions.fmg";
            tbx_file2_prepare.Text = @"C:\Sandbox\Modding data\DS_TMH\work in progress 1.3\FMG test files\Items - English - vanilla\Armor_long_desc_.fmg";
            tbx_file3_prepare.Text = @"C:\Sandbox\Modding data\DS_TMH\work in progress 1.3\FMG test files\Items - Italian - vanilla\Armor_long_desc_.fmg";
#endif


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



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            TabControl tabctrl = tbc_modes;
            TabItem selected = tabctrl.SelectedItem as TabItem;

            MessageBox.Show("Je suis situé sur l'onglet " + (tabctrl.SelectedIndex + 1) + " / " + tabctrl.Items.Count + " dont le Header répond au doux nom de '" + selected.Header.ToString() + "' dans le TabControl '" + tabctrl.Name + "'");

            //SortedDictionary<int, string> sdict = ReturnCompareDictionary(tbx_file1_compare.Text, tbx_file2_compare.Text, Tbx_CsvSeparator.Text[0], true);
            //DisplayPreviewCompare(sdict, Tbx_CsvSeparator.Text[0], tbx_header1.Text, tbx_header2.Text);

        }


        #region Compare mode

        private void tbx_file1_compare_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void tbx_file1_compare_Drop(object sender, DragEventArgs e)
        {
            if (AcceptDroppedInputFile(e))
                DisplayInputFilepath(sender, e);
        }

        private void tbx_file2_compare_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }

        private void tbx_file2_compare_Drop(object sender, DragEventArgs e)
        {
            if (AcceptDroppedInputFile(e))
                DisplayInputFilepath(sender, e);
        }



        private void Cbx_Cmp_UseInputFilename_Checked(object sender, RoutedEventArgs e)
        {
            Cmbx_Cmp_TargetInputFilename.IsEnabled = true;
            Cmbx_Cmp_TargetInputFilename.SelectedIndex = 0;
        }
        private void Cbx_Cmp_UseInputFilename_Unchecked(object sender, RoutedEventArgs e)
        {
            Cmbx_Cmp_TargetInputFilename.IsEnabled = false;
            Cmbx_Cmp_TargetInputFilename.SelectedIndex = -1;
        }



        private void tbx_header1_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateHeadersValues(sender);
        }
        private void tbx_header2_LostFocus(object sender, RoutedEventArgs e)
        {
            ValidateHeadersValues(sender);
        }

        private void Cbx_OneLinedValues_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void ValidateHeadersValues(object sender)
        {
            TextBox tbx = sender as TextBox;

            if (tbx.Text == "")
            {
                tbx.BorderBrush = Brushes.Red;
                MessageBox.Show("Value is required");
                return;
            }

            if (tbx.BorderBrush == Brushes.Red)
                tbx.ClearValue(BorderBrushProperty);
        }

        #endregion


        #region Prepare mode

        private void Cbx_Prp_UseInputFilename_Checked(object sender, RoutedEventArgs e)
        {
            Cmbx_Prp_TargetInputFilename.IsEnabled = true;
            Cmbx_Prp_TargetInputFilename.SelectedIndex = 2;
        }

        private void Cbx_Prp_UseInputFilename_Unchecked(object sender, RoutedEventArgs e)
        {
            Cmbx_Prp_TargetInputFilename.IsEnabled = false;
            Cmbx_Prp_TargetInputFilename.SelectedIndex = -1;
        }

        #endregion



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

            //btn_execute.IsEnabled = false;

            compare_clicked = true;
            prepare_clicked = false;

            /* TODO! Replace by external window
            tbk_summary.Text = "[Compare/Compile] To generate a new CSV file with data comparison\n"
                             + "Expected: File #1 is the modded file in {mod author's language}. File #2 is the same file in vanilla, in same language.\n"
                             + "The program will loop through contents of both files, scanning their entries:\n"
                             + "    For a given entry, do the files have a value and is it the same?\n"
                             + "        - None of them has a value skip: skip the entry = Write nothing in output file\n"
                             + "        - At least one of them has a value: compare both = Write \"True\" if identical or \"False\" if not";*/


            //if (CompareModeReady())
            //{
                //SortedDictionary<int, string> sdict = ReturnCompareDictionary(tbx_file1_compare.Text, tbx_file2_compare.Text, Tbx_CsvSeparator.Text[0], true);
                //DisplayPreviewCompare(sdict, Tbx_CsvSeparator.Text[0], tbx_header1.Text, tbx_header2.Text);

                //btn_execute.IsEnabled = true;
            //}
        }

        private void btn_preparefile_Click(object sender, RoutedEventArgs e)
        {
            // TODO: "Disable" useless controls for this mode : tbx_header1, tbx_header2

            //btn_execute.IsEnabled = false;

            compare_clicked = false;
            prepare_clicked = true;

            /* TODO! Replace by external window
            tbk_summary.Text = "[Prepare/Comply] To prepare a new FMG file for mod translation\n"
                             + "Expected: File #1 is the modded file in {mod author's language}. File #2 is the same file in vanilla, in same language. "
                             + "File #3 is the same file in vanilla, in {target new language}.\n"
                             + "The program will replicate structure of file #1, and fill it with values dynamically, depending on comparison between #1 and #2:\n"
                             + "    For each File #1 entry:\n"
                             + "        - Files #1 and #2 has identical values: use value from file #3, as this is the vanilla content and translation already exists\n"
                             + "        - Files #1 and #2 has different values: use value from file #1, and let the translator update it manually";
             */


            //if (PrepareModeReady())
            //{
                //Dictionary<int, string> dict = ReturnPrepareDictionary(tbx_file1.Text, tbx_file2.Text, tbx_file3.Text, true);
                //DisplayPreviewPrepare(dict, tbx_csvsepo.Text[0]);

                //btn_execute.IsEnabled = true;
            //}
        }




        private void btn_execute_Click(object sender, RoutedEventArgs e)
        {
            string output_filename = "";

            if (compare_clicked)
            {
                //SortedDictionary<int, string> sdict = ReturnCompareDictionary(tbx_file1_compare.Text, tbx_file2_compare.Text, Tbx_CsvSeparator.Text[0], false);
                //DoCompare(sdict, Tbx_CsvSeparator.Text[0], tbx_header1.Text, tbx_header2.Text, output_filename);
                MessageBox.Show($"Compare mode: File \"{output_filename}\" created.");
            }

            if (prepare_clicked)
            {
                //Dictionary<int, string> dict = ReturnPrepareDictionary(tbx_file1.Text, tbx_file2.Text, tbx_file3.Text, false);
                //DoPrepare(dict, tbx_csvsepo.Text[0], output_filename);
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
            //if (cbx_syncoutputfilename.IsChecked == false)
            //    return;

            //if (tbx_file1.Text == "Drop FMG file...")
            //    return;

            //FileInfo file_info = new FileInfo(tbx_file1.Text);
            //tbx_outputfilename.Text = file_info.Name.Substring(0, file_info.Name.Length - file_info.Extension.Length);
        }

        private bool CompareModeReady()
        {
            string input_filepath1 = tbx_file1_compare.Text;
            string input_filepath2 = tbx_file2_compare.Text;
            string output_header_1 = tbx_header1.Text;
            string output_header_2 = tbx_header2.Text;
            string output_filename = "";
            string sepa_csv_char_o = Tbx_CsvSeparator.Text;

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
            //string input_filepath1 = tbx_file1.Text;
            //string input_filepath2 = tbx_file2.Text;
            //string input_filepath3 = tbx_file3.Text;
            string output_filename = "";

            //if (!File.Exists(input_filepath1) || !File.Exists(input_filepath2) || !File.Exists(input_filepath3))
            //{
            //    MessageBox.Show("Error : missing input file(s)");
            //    return false;
            //}

            //if ((input_filepath1 == input_filepath2) || (input_filepath2 == input_filepath3) || (input_filepath1 == input_filepath3))
            //{
            //    MessageBox.Show("Error : same file submitted several times");
            //    return false;
            //}

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

            dtg_preview.ItemsSource = dictionary_compare;
            //tbk_preview.Text = output_preview;
        }

        private void DisplayPreviewPrepare(Dictionary<int, string> dictionary_prepare, char osep)
        {
            string output_preview = ""; // Headers = none
            foreach (KeyValuePair<int, string> od in dictionary_prepare)
            {
                output_preview += (output_preview == "") ? $"{od.Key}{osep}{od.Value}" : $"\n{od.Key}{osep}{od.Value}"; // Data = "Text ID|Value"
            }

            //tbk_preview.Text = output_preview;
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



        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            dtg_preview.Columns.Clear();

            //if (dtg_preview.ItemsSource != null)
            //{
            //    dtg_preview.ItemsSource = null;
                
            //}


            TabControl tabctrl = tbc_modes;

            //TabItem selected = tabctrl.SelectedItem as TabItem;

            if (tabctrl.SelectedIndex == 0)
            {
                string iFile1 = tbx_file1_compare.Text;
                string iFile2 = tbx_file2_compare.Text;
                //string oFilename = Tbx_Cmp_OutputFilename.Text;
                //char csvSeparator = Tbx_CsvSeparator.Text[0];
                CompareMode c = new CompareMode(iFile1, iFile2) { OneLinedValues = Cbx_OneLinedValues.IsChecked ?? false };

                //c.CheckFiles(); TODO!
                c.ProcessFiles(true);
                dtg_preview.Visibility = Visibility.Visible;
                List<CompareMode.Entry> cmp_entries = c.Entries;

                DataContext = cmp_entries;

                Style hdrOff = (Style)Grid_Main.Resources["MyStyle"];
                List<DataGridTextColumn> TextsList = new List<DataGridTextColumn>()
                {
                    new DataGridTextColumn() { Header = "#", HeaderStyle = hdrOff, Foreground = Brushes.Gray, Binding = new Binding("Index") },
                    new DataGridTextColumn() { Header = "Text ID", Binding = new Binding("TextId") },
                    new DataGridTextColumn() { Header = tbx_header1.Text, Binding = new Binding("Value1") },
                    new DataGridTextColumn() { Header = tbx_header2.Text, Binding = new Binding("Value2") },
                    new DataGridTextColumn() { Header = "Same?", Binding = new Binding("Same") }
                };

                foreach (DataGridTextColumn txtColumn in TextsList)
                {
                    dtg_preview.Columns.Add(txtColumn);
                }

                //DataGridTextColumn col = new DataGridTextColumn()
                //{
                //    Header = "#",
                //    Binding = new Binding("Index")
                //};
                //DataGridTextColumn col2 = new DataGridTextColumn()
                //{
                //    Header = "Text ID",
                //    Binding = new Binding("TextId")
                //};


                //dtg_preview.Columns.Add(col);
                //dtg_preview.Columns.Add(col2);

                //List<DataGridTemplateColumn> TemplatesList = new List<DataGridTemplateColumn>()
                //{
                //    new DataGridTemplateColumn() {
                //        //Header = "#",
                //        Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                //        CellTemplate = Grid_Main.Resources["KeyTry"] as DataTemplate
                //    },
                //    new DataGridTemplateColumn() {
                //        Header = "Text ID",
                //        Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                //        CellTemplate = Grid_Main.Resources["OnTemplate"] as DataTemplate
                //    },
                //    new DataGridTemplateColumn() {
                //        Header = "Header 1", // TODO: Sync with oHdr1
                //        Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                //        CellTemplate = Grid_Main.Resources["OnTemplate"] as DataTemplate
                //    },
                //    new DataGridTemplateColumn() {
                //        Header = "Header 2", // TODO: Sync with oHdr2
                //        Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                //        CellTemplate = Grid_Main.Resources["OnTemplate"] as DataTemplate
                //    },
                //    new DataGridTemplateColumn() {
                //        Header = "Same?", // TODO: Sync with oHdr2
                //        Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                //        CellTemplate = Grid_Main.Resources["OnTemplate"] as DataTemplate
                //    }
                //};

                //foreach (DataGridTemplateColumn template in TemplatesList)
                //{
                //    dtg_preview.Columns.Add(template);
                //}

                dtg_preview.ItemsSource = cmp_entries;

                // Play with all data (take the attributes you want)

                // ORIGINAL
                //dtg_preview.Columns.Add(new DataGridTemplateColumn()
                //{
                //    Header = "#",
                //    Width = new DataGridLength(1, DataGridLengthUnitType.Star),
                //    CellTemplate = FindResource("OffTemplate") as DataTemplate
                //});

                // var resource = Grid_Main.Resources["MyStyle"];


                //dtg_preview.Template.
                //DataGridTemplateColumn dgt = new DataGridTemplateColumn();

                //FrameworkElement resource = 
                //var resource = Grid_Main.Resources["OffTemplate"];
                //DataTemplate dt = (DataTemplate)resource;
                //string vear = "";


                //DataTemplate myTemplate = new DataTemplate();
                //myTemplate.DataType = new TextBlock();

                //FrameworkElementFactory factory = new FrameworkElementFactory(typeof(TextBlock));
                //factory.SetValue(TextBlock.ForegroundProperty = Foreground.);

                TextBlock tbk = new TextBlock()
                {
                    Foreground = Brushes.Gray
                };

                DataGridTemplateColumn offCol = new DataGridTemplateColumn();
                offCol.CellTemplate = (DataTemplate)Grid_Main.Resources["OffTemplate"];

                DataGridTemplateColumn onCol = new DataGridTemplateColumn()
                {
                    CellTemplate = (DataTemplate)Grid_Main.Resources["OnTemplate"]
                };

                //dtg_preview.Template = onCol
                //dtg_preview.Columns[0].HeaderTemplate = offCol;

                //DataGridTextColumn dtgTcol = new DataGridTextColumn()
                //{
                //    HeaderTemplate = 
                //}

                //FrameworkElementFactory factory = new FrameworkElementFactory(tbk);


                //dtg_preview.Columns.Add(dgt);

                //x: Name = "dtg_columns"

                //     DataGridTemplateColumn Header = "Publish Date"
                //     CellTemplate = "{StaticResource OffTemplate}"

                //dtg_preview.Columns[0].Header = "#";
                //dtg_preview.Columns[1].Header = "Text ID";
                //dtg_preview.Columns[2].Header = tbx_header1.Text;
                //dtg_preview.Columns[3].Header = tbx_header2.Text;
                //dtg_preview.Columns[4].Header = "Same?";

                //dtg_preview.Columns[0].MaxWidth = 50;
                //dtg_preview.Columns[1].MaxWidth = 75;

                //dtg_preview.Columns[1].MaxWidth = 540;
                //dtg_preview.Columns[1].MinWidth = 75;

                //dtg_preview.Columns[2].MaxWidth = 540;
                //dtg_preview.Columns[2].MinWidth = 75;
                //dtg_preview.Columns[3].MinWidth = 50;

                return;
            }
            //else
            PrepareMode p = new PrepareMode(tbx_file1_prepare.Text, tbx_file2_prepare.Text, tbx_file3_prepare.Text);
            
            p.ProcessFiles(false);
            List<PrepareMode.Entry> prp_Entries = p.Entries;

            dtg_preview.Visibility = Visibility.Visible;
            dtg_preview.ItemsSource = prp_Entries;

            dtg_preview.Columns[0].Header = "#";
            dtg_preview.Columns[1].Header = "Text ID";
            dtg_preview.Columns[2].Header = "File #1 value";
            dtg_preview.Columns[3].Header = "File #2 value";
            dtg_preview.Columns[4].Header = "File #3 value";
            dtg_preview.Columns[5].Header = "Output value";
            dtg_preview.Columns[6].Header = "From?";

            dtg_preview.Columns[0].MaxWidth = 50;

            DataTemplate dtemplate = new DataTemplate();
            DataGridTemplateColumn dtgTemplateCol = new DataGridTemplateColumn();
            //dtg_preview.Columns[0].HeaderTemplate = dtgTemplateCol;

            dtg_preview.Columns[1].MaxWidth = 75;

            dtg_preview.Columns[2].MinWidth = 75;
            dtg_preview.Columns[2].MaxWidth = 260;
            dtg_preview.Columns[3].MinWidth = 75;
            dtg_preview.Columns[3].MaxWidth = 260;
            dtg_preview.Columns[4].MinWidth = 75;
            dtg_preview.Columns[4].MaxWidth = 260;
            dtg_preview.Columns[5].MinWidth = 75;
            dtg_preview.Columns[5].MaxWidth = 260;

            dtg_preview.Columns[6].MaxWidth = 50;

            return;
            //dtg_preview.Columns[1].MinWidth = dtg_preview.Columns[0].Header.ToString().Length;
            //dtg_preview.Columns[1].MinWidth = (double)DataGridLengthUnitType.SizeToHeader;

            //SortedDictionary<int, string> sdict = ReturnCompareDictionary(tbx_file1_compare.Text, tbx_file2_compare.Text, tbx_csvsepo.Text[0], true);
            //DisplayPreviewCompare(sdict, tbx_csvsepo.Text[0], tbx_header1.Text, tbx_header2.Text);

            //List<int> id_list = new List<int>() { 1, 2, 3, 4, 5 };
            //List<string> val_list = new List<string>() { "Casque", "Plastron", "Gantelets", "Jambières", "Slip" };

            // WORKING SIMPLE
            //Dictionary<int, string> dict = new Dictionary<int, string>()
            //{
            //    { 1, "Casque" },
            //    { 2, "Plastron" },
            //    { 3, "Gantelets" },
            //    { 4, "Jambières" },
            //    { 5, "Slip" }
            //};
            //dtg_preview.ItemsSource = dict;




            // MAYBE
            //Dictionary<int, List<string>> cmp_dictionary = new Dictionary<int, List<string>>();
            //cmp_dictionary.Add(123, new List<string>() { "File 1 - value 1", "File 2 - value 1" });
            //cmp_dictionary.Add(456, new List<string>() { "File 1 - value 2", "File 2 - value 2" });
            //cmp_dictionary.Add(789, new List<string>() { "File 1 - value 3", "File 2 - value 3" });

            //dtg_preview.ItemsSource = cmp_dictionary;




            // OK GREAT
            //List<Entry> entries = new List<Entry>()
            //{
            //    new Entry(123, "Casque Le Bohom", "Armure dentelée", "false"),
            //    new Entry(234, "Épée Durendil", "Lame de phoque", "false"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true"),
            //    new Entry(345, "Vestige ancien", "Vestige ancien", "true")
            //};
            ////entries.Add(new Entry(123, "Casque Le Bohom", "Armure dentelée", false));
            ////entries.Add(new Entry(234, "Épée Durendil", "Lame de phoque", false));
            ////entries.Add(new Entry(345, "Vestige ancien", "Vestige ancien", true));

            //string oHdr1 = "File 1 value";
            //string oHdr2 = "File 2 value";

            //dtg_preview.Visibility = Visibility.Visible;

            //dtg_preview.ItemsSource = entries;
            //dtg_preview.Columns[0].Header = "Text ID";
            //dtg_preview.Columns[1].Header = oHdr1;
            //dtg_preview.Columns[2].Header = oHdr2;
            //dtg_preview.Columns[3].Header = "Same?";
        }


        private void Btn_ClearPreview_Click(object sender, RoutedEventArgs e)
        {
            if (dtg_preview.Visibility == Visibility.Visible && dtg_preview.ItemsSource != null)
            {
                dtg_preview.ItemsSource = null;
                dtg_preview.Visibility = Visibility.Hidden;
                //dtg_preview.Columns.Clear();
                //dtg_preview.Items.Clear();
                //dtg_preview.Items.Refresh();
            }
        }

        private void Btn_DetachPreview_Click(object sender, RoutedEventArgs e)
        {
            if (dtg_preview.Visibility == Visibility.Visible && dtg_preview.ItemsSource != null)
            {
                // Detach preview to external new window
            }
        }

        private void Cbx_PreviewAllDetails_Checked(object sender, RoutedEventArgs e)
        {
            // Reload preview, including index and other extra columns
        }

        private void Btn_RefreshPreview_Click(object sender, RoutedEventArgs e)
        {
            dtg_preview.Visibility = Visibility.Visible;
            dtg_preview.Columns.Clear();

            if (tbc_modes.SelectedIndex == 0)
            {

                string iFile1 = tbx_file1_compare.Text;
                string iFile2 = tbx_file2_compare.Text;
                CompareMode c = new CompareMode(iFile1, iFile2) { OneLinedValues = Cbx_OneLinedValues.IsChecked ?? false };

                //c.CheckFiles(); TODO!
                c.ProcessFiles(true);
                //List<CompareMode.Entry> cmp_entries = c.Entries;
                DataContext = c.Entries;
                //_ = MessageBox.Show($"What the damn is DataContext ? = {DataContext}");

                //Style hdrOff = (Style)Grid_Main.Resources["HeaderOff"]
                List<DataGridTextColumn> TextsColumns = new List<DataGridTextColumn>()
                {
                    new DataGridTextColumn() {
                        Header = "#",
                        HeaderStyle = (Style)Grid_Main.Resources["HeaderOff"],
                        Foreground = Brushes.Gray,
                        Binding = new Binding("Index")
                    },
                    new DataGridTextColumn() {
                        Header = "Text ID",
                        Binding = new Binding("TextId")
                    },
                    new DataGridTextColumn() {
                        Header = tbx_header1.Text,
                        Binding = new Binding("Value1")
                    },
                    new DataGridTextColumn() {
                        Header = tbx_header2.Text,
                        Binding = new Binding("Value2")
                    },
                    new DataGridTextColumn() {
                        Header = "Same?",
                        Binding = new Binding("Same")
                    }
                };
                foreach (DataGridTextColumn column in TextsColumns)
                {
                    dtg_preview.Columns.Add(column);
                }
                dtg_preview.ItemsSource = c.Entries;

                //dtg_preview.Columns[0].Header = "#";
                //dtg_preview.Columns[1].Header = "Text ID";
                //dtg_preview.Columns[2].Header = tbx_header1.Text;
                //dtg_preview.Columns[3].Header = tbx_header2.Text;
                //dtg_preview.Columns[4].Header = "Same?";

                //dtg_preview.Columns[0].MaxWidth = 50;
                //dtg_preview.Columns[1].MaxWidth = 75;

                //dtg_preview.Columns[1].MaxWidth = 540;
                //dtg_preview.Columns[1].MinWidth = 75;

                //dtg_preview.Columns[2].MaxWidth = 540;
                //dtg_preview.Columns[2].MinWidth = 75;
                //dtg_preview.Columns[3].MinWidth = 50;
            }
            else
            {
                string iFile1 = tbx_file1_prepare.Text;
                string iFile2 = tbx_file2_prepare.Text;
                string iFile3 = tbx_file3_prepare.Text;

                PrepareMode p = new PrepareMode(iFile1, iFile2, iFile3);

                p.ProcessFiles(false);
                DataContext = p.Entries;
                //List<PrepareMode.Entry> prp_Entries = p.Entries;

                List<DataGridTextColumn> TextsColumns = new List<DataGridTextColumn>()
                {
                    new DataGridTextColumn() {
                        Header = "#",
                        HeaderStyle = (Style)Grid_Main.Resources["HeaderOff"],
                        Foreground = Brushes.Gray,
                        Binding = new Binding("Index")
                    },
                    new DataGridTextColumn() {
                        Header = "Text ID",
                        Binding = new Binding("TextId")
                    },
                    new DataGridTextColumn() {
                        Header = "Value 1",
                        HeaderStyle = (Style)Grid_Main.Resources["HeaderOff"],
                        Foreground = Brushes.Gray,
                        Binding = new Binding("Value1")
                    },
                    new DataGridTextColumn() {
                        Header = "Value 2",
                        HeaderStyle = (Style)Grid_Main.Resources["HeaderOff"],
                        Foreground = Brushes.Gray,
                        Binding = new Binding("Value2")
                    },
                    new DataGridTextColumn() {
                        Header = "Value 3",
                        HeaderStyle = (Style)Grid_Main.Resources["HeaderOff"],
                        Foreground = Brushes.Gray,
                        Binding = new Binding("Value3")
                    },
                    new DataGridTextColumn() {
                        Header = "Output value",
                        Binding = new Binding("Output")
                    },
                    new DataGridTextColumn() {
                        Header = "From?",
                        HeaderStyle = (Style)Grid_Main.Resources["HeaderOff"],
                        Foreground = Brushes.Gray,
                        Binding = new Binding("Source")
                    }
                };
                foreach (DataGridTextColumn column in TextsColumns)
                {
                    dtg_preview.Columns.Add(column);
                }
                dtg_preview.ItemsSource = p.Entries;

                // NEXT
                //dtg_preview.ItemsSource = p.Entries;

                //dtg_preview.Columns[0].Header = "#";
                //dtg_preview.Columns[1].Header = "Text ID";
                //dtg_preview.Columns[2].Header = "File #1 value";
                //dtg_preview.Columns[3].Header = "File #2 value";
                //dtg_preview.Columns[4].Header = "File #3 value";
                //dtg_preview.Columns[5].Header = "Output value";
                //dtg_preview.Columns[6].Header = "From?";

                //dtg_preview.Columns[0].MaxWidth = 50;
                //dtg_preview.Columns[1].MaxWidth = 75;
                //dtg_preview.Columns[2].MinWidth = 75;
                //dtg_preview.Columns[2].MaxWidth = 260;
                //dtg_preview.Columns[3].MinWidth = 75;
                //dtg_preview.Columns[3].MaxWidth = 260;
                //dtg_preview.Columns[4].MinWidth = 75;
                //dtg_preview.Columns[4].MaxWidth = 260;
                //dtg_preview.Columns[5].MinWidth = 75;
                //dtg_preview.Columns[5].MaxWidth = 260;
                //dtg_preview.Columns[6].MaxWidth = 50;
            }
        }

        private List<DataGridTextColumn> BuildDataGridTextColumns(FrameworkElement DataContext, string mode)
        {
            List<DataGridTextColumn> ColumnsList = new List<DataGridTextColumn>();

            switch (mode)
            {
                case "Compare":
                    ColumnsList.Add(new DataGridTextColumn()
                    {
                        Header = "#",
                        HeaderStyle = (Style)Grid_Main.Resources["HeaderOff"],
                        Foreground = Brushes.Gray,
                        Binding = new Binding("Index")
                    });
                    
                    break;

                case "Prepare":

                    break;

                default:
                    // Unkown, man you did wrong
                    break;
            }
            return ColumnsList;
        }

    }

}
