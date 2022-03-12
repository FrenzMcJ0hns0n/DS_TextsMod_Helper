using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DS_TextsMod_Helper
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Directory.CreateDirectory(IOHelper.GetOutputDirPath());

            if (!File.Exists(Path.Combine(IOHelper.GetRootDirPath(), "SoulsFormats.dll")))
                MessageBox.Show("Fatal error : file 'SoulsFormats.dll' not found");

#if DEBUG
            Tbx_Cmp_iFile1.Text = @"C:\Sandbox\Modding data\DS_TMH\published 1.2\FMG test files\English - Daughters of Ash\RingDescriptions.fmg";
            Tbx_Cmp_iFile2.Text = @"C:\Sandbox\Modding data\DS_TMH\published 1.2\FMG test files\English - vanilla\Accessory_long_desc_.fmg";

            //Tbx_Prp_iFile1.Text = @"C:\Sandbox\Modding data\DS_TMH\published 1.2\FMG test files\English - Daughters of Ash\RingDescriptions.fmg";
            //Tbx_Prp_iFile2.Text = @"C:\Sandbox\Modding data\DS_TMH\published 1.2\FMG test files\English - vanilla\Accessory_long_desc_.fmg";
            //Tbx_Prp_iFile3.Text = @"C:\Sandbox\Modding data\DS_TMH\published 1.2\FMG test files\French - vanilla\Accessory_long_desc_.fmg";

            //Tbx_Cmp_iFile1.Text = @"C:\Sandbox\Modding data\DS_TMH\work in progress 1.3\FMG test files\Items - English - vanilla\Item_name_.fmg";
            //Tbx_Cmp_iFile2.Text = @"C:\Sandbox\Modding data\DS_TMH\work in progress 1.3\FMG test files\Items - English - Daughters of Ash\ItemNames.fmg";

            Tbx_Prp_iFile1.Text = @"C:\Sandbox\Modding data\DS_TMH\work in progress 1.3\FMG test files\Items - English - Daughters of Ash\ArmorDescriptions.fmg";
            Tbx_Prp_iFile2.Text = @"C:\Sandbox\Modding data\DS_TMH\work in progress 1.3\FMG test files\Items - English - vanilla\Armor_long_desc_.fmg";
            Tbx_Prp_iFile3.Text = @"C:\Sandbox\Modding data\DS_TMH\work in progress 1.3\FMG test files\Items - Italian - vanilla\Armor_long_desc_.fmg";
#endif
        }


        #region GUI Compare mode

        private void Tbx_Cmp_iFile1_PreviewDragOver(object sender, DragEventArgs e) { e.Handled = true; }
        private void Tbx_Cmp_iFile1_Drop(object sender, DragEventArgs e)
        {
            if (AcceptDroppedInputFile(e))
            {
                DisplayInputFilepath(sender, e);
                SyncFilenames(sender);
            }
        }
        private void Tbx_Cmp_iFile2_PreviewDragOver(object sender, DragEventArgs e) { e.Handled = true; }
        private void Tbx_Cmp_iFile2_Drop(object sender, DragEventArgs e)
        {
            if (AcceptDroppedInputFile(e))
            {
                DisplayInputFilepath(sender, e);
                SyncFilenames(sender);
            }
        }
        private void Tbx_Cmp_oFilename_GotFocus(object sender, RoutedEventArgs e) { SelectTbxValue(sender); }
        private void Tbx_Cmp_oFilename_LostFocus(object sender, RoutedEventArgs e) { ValidateTbxValue(sender); }
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
        private void Cmbx_Cmp_TargetInputFilename_SelectionChanged(object sender, SelectionChangedEventArgs e) { SyncFilenames(sender); }
        private void Tbx_Cmp_oHeader1_GotFocus(object sender, RoutedEventArgs e) { SelectTbxValue(sender); }
        private void Tbx_Cmp_oHeader1_LostFocus(object sender, RoutedEventArgs e) { ValidateTbxValue(sender); }
        private void Tbx_Cmp_oHeader2_GotFocus(object sender, RoutedEventArgs e) { SelectTbxValue(sender); }
        private void Tbx_Cmp_oHeader2_LostFocus(object sender, RoutedEventArgs e) { ValidateTbxValue(sender); }
        private void Tbx_Cmp_CsvSeparator_GotFocus(object sender, RoutedEventArgs e) { SelectTbxValue(sender); }
        private void Tbx_Cmp_CsvSeparator_LostFocus(object sender, RoutedEventArgs e) { ValidateTbxValue(sender); }

        #endregion


        #region GUI Prepare mode

        private void Tbx_Prp_iFile1_PreviewDragOver(object sender, DragEventArgs e) { e.Handled = true; }
        private void Tbx_Prp_iFile1_Drop(object sender, DragEventArgs e)
        {
            if (AcceptDroppedInputFile(e))
            {
                DisplayInputFilepath(sender, e);
                SyncFilenames(sender);
            }
        }
        private void Tbx_Prp_iFile2_PreviewDragOver(object sender, DragEventArgs e) { e.Handled = true; }
        private void Tbx_Prp_iFile2_Drop(object sender, DragEventArgs e)
        {
            if (AcceptDroppedInputFile(e))
            {
                DisplayInputFilepath(sender, e);
                SyncFilenames(sender);
            }
        }
        private void Tbx_Prp_iFile3_PreviewDragOver(object sender, DragEventArgs e) { e.Handled = true; }
        private void Tbx_Prp_iFile3_Drop(object sender, DragEventArgs e)
        {
            if (AcceptDroppedInputFile(e))
            {
                DisplayInputFilepath(sender, e);
                SyncFilenames(sender);
            }
        }
        private void Tbx_Prp_oFilename_GotFocus(object sender, RoutedEventArgs e) { SelectTbxValue(sender); }
        private void Tbx_Prp_oFilename_LostFocus(object sender, RoutedEventArgs e) { ValidateTbxValue(sender); }
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
        private void Cmbx_Prp_TargetInputFilename_SelectionChanged(object sender, SelectionChangedEventArgs e) { SyncFilenames(sender); }
        private void Tbx_Prp_TextToReplace_GotFocus(object sender, RoutedEventArgs e) { SelectTbxValue(sender); }
        private void Tbx_Prp_ReplacingText_GotFocus(object sender, RoutedEventArgs e) { SelectTbxValue(sender); }

        #endregion


        #region GUI Helpers

        private bool AcceptDroppedInputFile(DragEventArgs e)
        {
            if (!(e.Data.GetData(DataFormats.FileDrop) is string[]))
                return false;

            string path = ((string[])e.Data.GetData(DataFormats.FileDrop))[0];
            if (!File.Exists(path))
                return false;

            if (new FileInfo(path).Extension.ToLowerInvariant() != ".fmg")
                return false;

            try { _ = SoulsFormats.FMG.Read(path); }
            catch (Exception ex)
            {
                _ = MessageBox.Show("Error while reading this input file : '" + ex.ToString() + "'");
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

        private void SelectTbxValue(object sender)
        {
            TextBox tbx = sender as TextBox;
            tbx.SelectAll();
        }

        private void ValidateTbxValue(object sender)
        {
            TextBox tbx = sender as TextBox;

            if (tbx.Text == "")
                tbx.BorderBrush = Brushes.Red;
            else
                tbx.ClearValue(BorderBrushProperty);
        }

        private void SyncFilenames(object sender) //TODO: Factorize
        {
            if (sender is TextBox)
            {
                TextBox tbx = sender as TextBox;

                if (tbx.Name.Contains("Cmp") && (Cbx_Cmp_UseInputFilename.IsChecked ?? true)) // Sender is TextBox from Compare mode
                {
                    if (Cmbx_Cmp_TargetInputFilename.SelectedIndex == 0 && tbx == Tbx_Cmp_iFile1)
                        Tbx_Cmp_oFilename.Text = new FileInfo(Tbx_Cmp_iFile1.Text).Name;
                    else if (Cmbx_Cmp_TargetInputFilename.SelectedIndex == 1 && tbx == Tbx_Cmp_iFile2)
                        Tbx_Cmp_oFilename.Text = new FileInfo(Tbx_Cmp_iFile2.Text).Name;

                    ValidateTbxValue(Tbx_Cmp_oFilename);
                }
                if (tbx.Name.Contains("Prp") && (Cbx_Prp_UseInputFilename.IsChecked ?? true)) // Sender is TextBox from Prepare mode
                {
                    if (Cmbx_Prp_TargetInputFilename.SelectedIndex == 0 && tbx == Tbx_Prp_iFile1)
                        Tbx_Prp_oFilename.Text = new FileInfo(Tbx_Prp_iFile1.Text).Name;
                    else if (Cmbx_Prp_TargetInputFilename.SelectedIndex == 1 && tbx == Tbx_Prp_iFile2)
                        Tbx_Prp_oFilename.Text = new FileInfo(Tbx_Prp_iFile2.Text).Name;
                    else if (Cmbx_Prp_TargetInputFilename.SelectedIndex == 2 && tbx == Tbx_Prp_iFile3)
                        Tbx_Prp_oFilename.Text = new FileInfo(Tbx_Prp_iFile3.Text).Name;

                    ValidateTbxValue(Tbx_Prp_oFilename);
                }
            }

            if (sender is ComboBox)
            {
                ComboBox cmbx = sender as ComboBox;

                if (cmbx == Cmbx_Cmp_TargetInputFilename) // Sender is ComboBox from Compare mode
                {
                    if (cmbx.SelectedIndex == 0 && Tbx_Cmp_iFile1.Text != "Drop FMG file...")
                        Tbx_Cmp_oFilename.Text = new FileInfo(Tbx_Cmp_iFile1.Text).Name;
                    else if (cmbx.SelectedIndex == 1 && Tbx_Cmp_iFile2.Text != "Drop FMG file...")
                        Tbx_Cmp_oFilename.Text = new FileInfo(Tbx_Cmp_iFile2.Text).Name;

                    ValidateTbxValue(Tbx_Cmp_oFilename);
                }
                if (cmbx == Cmbx_Prp_TargetInputFilename) // Sender is ComboBox from Prepare mode
                {
                    if (cmbx.SelectedIndex == 0 && Tbx_Prp_iFile1.Text != "Drop FMG file...")
                        Tbx_Prp_oFilename.Text = new FileInfo(Tbx_Prp_iFile1.Text).Name;
                    else if (cmbx.SelectedIndex == 1 && Tbx_Prp_iFile2.Text != "Drop FMG file...")
                        Tbx_Prp_oFilename.Text = new FileInfo(Tbx_Prp_iFile2.Text).Name;
                    else if (cmbx.SelectedIndex == 2 && Tbx_Prp_iFile3.Text != "Drop FMG file...")
                        Tbx_Prp_oFilename.Text = new FileInfo(Tbx_Prp_iFile3.Text).Name;

                    ValidateTbxValue(Tbx_Prp_oFilename);
                }
            }
        }

        #endregion


        #region Preview

        private void Btn_RefreshPreview_Click(object sender, RoutedEventArgs e)
        {
            if (Tbc_Modes.SelectedIndex == 0)
            {
                string iFile1 = Tbx_Cmp_iFile1.Text;
                string iFile2 = Tbx_Cmp_iFile2.Text;

                if (iFile1 == "" || iFile2 == "")
                {
                    _ = MessageBox.Show("Error : Missing input file(s)");
                    return;
                }

                CompareMode c = new CompareMode(iFile1, iFile2) { OneLinedValues = Cbx_Cmp_OneLinedValues.IsChecked ?? false };
                c.ProcessFiles(true);

                bool allDetails = Cbx_PreviewAllDetails.IsChecked ?? false;
                PreviewCompare(c.Entries, allDetails);
            }
            else
            {
                string iFile1 = Tbx_Prp_iFile1.Text;
                string iFile2 = Tbx_Prp_iFile2.Text;
                string iFile3 = Tbx_Prp_iFile3.Text;
                string textToReplace = Tbx_Prp_TextToReplace.Text;
                string replacingText = Tbx_Prp_ReplacingText.Text;

                if (iFile1 == "" || iFile2 == "" || iFile3 == "")
                {
                    _ = MessageBox.Show("Error : Missing input file(s)");
                    return;
                }

                PrepareMode p = new PrepareMode(iFile1, iFile2, iFile3, textToReplace, replacingText);
                p.ProcessFiles(true);

                bool allDetails = Cbx_PreviewAllDetails.IsChecked ?? false;
                PreviewPrepare(p.Entries, allDetails);
            }
        }

        private void PreviewCompare(List<CompareMode.Entry> c_entries, bool allDetails)
        {
            Dtg_Preview.Visibility = Visibility.Visible;
            Dtg_Preview.Columns.Clear();

            DataContext = c_entries;

            string oHeader1 = Tbx_Cmp_oHeader1.Text != "" ? Tbx_Cmp_oHeader1.Text : "Header #1";
            string oHeader2 = Tbx_Cmp_oHeader2.Text != "" ? Tbx_Cmp_oHeader2.Text : "Header #2";

            List<DataGridTextColumn> columns = new List<DataGridTextColumn>()
            {
                new DataGridTextColumn() { Header = "Text ID", Binding = new Binding("TextId"), MaxWidth = 75 },
                new DataGridTextColumn() { Header = oHeader1, Binding = new Binding("Value1"), MaxWidth = 500 },
                new DataGridTextColumn() { Header = oHeader2, Binding = new Binding("Value2"), MaxWidth = 500 },
                new DataGridTextColumn() { Header = "Same?", Binding = new Binding("Same"), MaxWidth = 50 }
            };

            if (allDetails)
            {
                Style hdrOff = (Style)Grid_Main.Resources["HeaderOff"]; //Style style = new Style() // TODO? See how to do this from code behind
                columns.Insert(0, new DataGridTextColumn() { Header = "#", HeaderStyle = hdrOff, Foreground = Brushes.Gray, Binding = new Binding("Index"), MaxWidth = 30 });
            }

            foreach (DataGridTextColumn col in columns)
                Dtg_Preview.Columns.Add(col);

            Dtg_Preview.ItemsSource = c_entries;
        }

        private void PreviewPrepare(List<PrepareMode.Entry> p_entries, bool allDetails)
        {
            Dtg_Preview.Visibility = Visibility.Visible;
            Dtg_Preview.Columns.Clear();

            DataContext = p_entries;

            List<DataGridTextColumn> columns = new List<DataGridTextColumn>()
            {
                new DataGridTextColumn() { Header = "Text ID", Binding = new Binding("TextId"), MaxWidth = 75 },
                new DataGridTextColumn() { Header = "Output value", Binding = new Binding("Output"), MaxWidth = allDetails ? 250 : 500 }
            };

            if (allDetails)
            {
                Style hdrOff = (Style)Grid_Main.Resources["HeaderOff"];
                columns.Insert(0, new DataGridTextColumn() { Header = "#", HeaderStyle = hdrOff, Foreground = Brushes.Gray, Binding = new Binding("Index"), MaxWidth = 30 });
                columns.Insert(2, new DataGridTextColumn() { Header = "File 1 value", HeaderStyle = hdrOff, Foreground = Brushes.Gray, Binding = new Binding("Value1"), MaxWidth = 250 });
                columns.Insert(3, new DataGridTextColumn() { Header = "File 2 value", HeaderStyle = hdrOff, Foreground = Brushes.Gray, Binding = new Binding("Value2"), MaxWidth = 250 });
                columns.Insert(4, new DataGridTextColumn() { Header = "File 3 value", HeaderStyle = hdrOff, Foreground = Brushes.Gray, Binding = new Binding("Value3"), MaxWidth = 250 });
                columns.Insert(6, new DataGridTextColumn() { Header = "From?", HeaderStyle = hdrOff, Foreground = Brushes.Gray, Binding = new Binding("Source"), MaxWidth = 50 });
            }

            foreach (DataGridTextColumn col in columns)
                Dtg_Preview.Columns.Add(col);

            Dtg_Preview.ItemsSource = p_entries;
        }

        private void Btn_ClearPreview_Click(object sender, RoutedEventArgs e)
        {
            if (Dtg_Preview.Visibility == Visibility.Visible && Dtg_Preview.ItemsSource != null)
            {
                Dtg_Preview.Visibility = Visibility.Hidden;
                Dtg_Preview.ItemsSource = null;
            }
        }

        private void Btn_DetachPreview_Click(object sender, RoutedEventArgs e)
        {
            if (Dtg_Preview.Visibility == Visibility.Visible && Dtg_Preview.ItemsSource != null)
            {
                // Detach preview to external new window
            }
        }

        private void Cbx_PreviewAllDetails_Checked(object sender, RoutedEventArgs e)
        {
            if (Dtg_Preview.Visibility == Visibility.Visible)
                Btn_RefreshPreview_Click(sender, e);
        }

        private void Cbx_PreviewAllDetails_Unchecked(object sender, RoutedEventArgs e)
        {
            if (Dtg_Preview.Visibility == Visibility.Visible)
                Btn_RefreshPreview_Click(sender, e);
        }

        #endregion


        #region Output

        private void Btn_GenerateOutput_Click(object sender, RoutedEventArgs e)
        {
            if (Tbc_Modes.SelectedIndex == 0)
            {
                if (!ValidateCompareInputs())
                    return;

                string iFile1 = Tbx_Cmp_iFile1.Text;
                string iFile2 = Tbx_Cmp_iFile2.Text;
                string oFilename = Tbx_Cmp_oFilename.Text + ".csv";
                string oHdr1 = Tbx_Cmp_oHeader1.Text;
                string oHdr2 = Tbx_Cmp_oHeader2.Text;
                string csvSepChar = Tbx_Cmp_CsvSeparator.Text;

                CompareMode c = new CompareMode(iFile1, iFile2) { OneLinedValues = Cbx_Cmp_OneLinedValues.IsChecked ?? false };
                c.ProcessFiles(false);
                c.ProduceOutput(oFilename, oHdr1, oHdr2, csvSepChar);

                _ = MessageBox.Show($"[Compare mode] File \"{c.OutputFilename}\" created");
            }
            else
            {
                if (!ValidatePrepareInputs())
                    return;

                string iFile1 = Tbx_Prp_iFile1.Text;
                string iFile2 = Tbx_Prp_iFile2.Text;
                string iFile3 = Tbx_Prp_iFile3.Text;
                string oFilename = Tbx_Prp_oFilename.Text + ".fmg";
                string textToReplace = Tbx_Prp_TextToReplace.Text;
                string replacingText = Tbx_Prp_ReplacingText.Text;

                PrepareMode p = new PrepareMode(iFile1, iFile2, iFile3, textToReplace, replacingText);
                p.ProcessFiles(false);
                p.ProduceOutput(oFilename);

                _ = MessageBox.Show($"[Prepare mode] File \"{p.OutputFilename}\" created");
            }
        }

        private bool ValidateCompareInputs()
        {
            List<string> errors = new List<string>();

            string input_filepath1 = Tbx_Cmp_iFile1.Text;
            string input_filepath2 = Tbx_Cmp_iFile2.Text;
            string output_header_1 = Tbx_Cmp_oHeader1.Text;
            string output_header_2 = Tbx_Cmp_oHeader2.Text;
            string output_filename = Tbx_Cmp_oFilename.Text;
            string sepa_csv_char_o = Tbx_Cmp_CsvSeparator.Text;

            if (!File.Exists(input_filepath1) || !File.Exists(input_filepath2))
                errors.Add("Error : Missing input file(s)");

            if (input_filepath1 == input_filepath2)
                errors.Add("Error : Same file submitted twice");

            if (output_header_1 == "" || output_header_2 == "")
                errors.Add("Error : Missing output header(s)");

            if (output_filename == "")
                errors.Add("Error : Missing output filename");

            if (sepa_csv_char_o == "")
                errors.Add("Error : Missing CSV separator");

            if (errors.Count > 0)
            {
                _ = MessageBox.Show(string.Join("\n\n", errors));
                return false;
            }
            return true;
        }

        private bool ValidatePrepareInputs()
        {
            List<string> errors = new List<string>();

            string input_filepath1 = Tbx_Prp_iFile1.Text;
            string input_filepath2 = Tbx_Prp_iFile2.Text;
            string input_filepath3 = Tbx_Prp_iFile3.Text;
            string output_filename = Tbx_Prp_oFilename.Text;

            if (!File.Exists(input_filepath1) || !File.Exists(input_filepath2) || !File.Exists(input_filepath3))
                errors.Add("Error : Missing input file(s)");

            if ((input_filepath1 == input_filepath2) || (input_filepath2 == input_filepath3) || (input_filepath1 == input_filepath3))
                errors.Add("Error : Same file submitted several times");

            if (output_filename == "")
                errors.Add("Error : Missing output filename");

            if (errors.Count > 0)
            {
                _ = MessageBox.Show(string.Join("\n\n", errors));
                return false;
            }
            return true;
        }

        #endregion


    }

}
