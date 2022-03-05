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



        private void Btn_ClearPreview_Click(object sender, RoutedEventArgs e)
        {
            if (dtg_preview.Visibility == Visibility.Visible && dtg_preview.ItemsSource != null)
            {
                dtg_preview.Visibility = Visibility.Hidden;
                dtg_preview.ItemsSource = null;
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
                        Binding = new Binding("Index"),
                        MaxWidth = 30
                    },
                    new DataGridTextColumn() {
                        Header = "Text ID",
                        Binding = new Binding("TextId"),
                        MaxWidth = 70
                    },
                    new DataGridTextColumn() {
                        Header = tbx_header1.Text,
                        Binding = new Binding("Value1"),
                        MaxWidth = 500
                    },
                    new DataGridTextColumn() {
                        Header = tbx_header2.Text,
                        Binding = new Binding("Value2"),
                        MaxWidth = 500
                    },
                    new DataGridTextColumn() {
                        Header = "Same?",
                        Binding = new Binding("Same"),
                        MaxWidth = 50
                    }
                };
                foreach (DataGridTextColumn column in TextsColumns)
                    dtg_preview.Columns.Add(column);

                dtg_preview.ItemsSource = c.Entries;
                //dtg_preview.ItemsSource = c.Entries;

                //dtg_preview.Columns[0].Header = "#";
                //dtg_preview.Columns[1].Header = "Text ID";
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
                        Binding = new Binding("Index"),
                        MaxWidth = 30
                    },
                    new DataGridTextColumn() {
                        Header = "Text ID",
                        Binding = new Binding("TextId"),
                        MaxWidth = 70
                    },
                    new DataGridTextColumn() {
                        Header = "Value 1",
                        HeaderStyle = (Style)Grid_Main.Resources["HeaderOff"],
                        Foreground = Brushes.Gray,
                        Binding = new Binding("Value1"),
                        MaxWidth = 250
                    },
                    new DataGridTextColumn() {
                        Header = "Value 2",
                        HeaderStyle = (Style)Grid_Main.Resources["HeaderOff"],
                        Foreground = Brushes.Gray,
                        Binding = new Binding("Value2"),
                        MaxWidth = 250
                    },
                    new DataGridTextColumn() {
                        Header = "Value 3",
                        HeaderStyle = (Style)Grid_Main.Resources["HeaderOff"],
                        Foreground = Brushes.Gray,
                        Binding = new Binding("Value3"),
                        MaxWidth = 250
                    },
                    new DataGridTextColumn() {
                        Header = "Output value",
                        Binding = new Binding("Output"),
                        MaxWidth = 250
                    },
                    new DataGridTextColumn() {
                        Header = "From?",
                        HeaderStyle = (Style)Grid_Main.Resources["HeaderOff"],
                        Foreground = Brushes.Gray,
                        Binding = new Binding("Source"),
                        MaxWidth = 50
                    }
                };
                foreach (DataGridTextColumn column in TextsColumns)
                    dtg_preview.Columns.Add(column);

                dtg_preview.ItemsSource = p.Entries;
                //dtg_preview.ItemsSource = p.Entries;

                //dtg_preview.Columns[2].Header = "File #1 value";
                //dtg_preview.Columns[2].MinWidth = 75;
                //dtg_preview.Columns[2].MaxWidth = 260;
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

        private void Btn_GenerateOutput_Click(object sender, RoutedEventArgs e)
        {
            // Validate inputs before going
        }

    }

}
