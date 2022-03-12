using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;

namespace DS_TextsMod_Helper
{
    public partial class MainWindow : Window
    {

        #region CONSTANTS

        private const string TBX_DEFAULT = "Drop FMG file...";

        private const string ERR_MISSING_IFILES = "Error : Missing input file(s)";
        private const string ERR_MISSING_OFNAME = "Error : Missing output filename";
        private const string ERR_MISSING_OHDRS = "Error : Missing output header(s)";
        private const string ERR_MISSING_CSVSEP = "Error : Missing CSV separator";
        private const string ERR_SAME_IFILE = "Error : Same file submitted several times";

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            Directory.CreateDirectory(IOHelper.GetOutputDirPath());

            if (!File.Exists(Path.Combine(IOHelper.GetRootDirPath(), "SoulsFormats.dll")))
                MessageBox.Show("Fatal error : file 'SoulsFormats.dll' not found");

#if DEBUG
            //Tbx_Cmp_iFile1.Text = @"C:\Sandbox\Modding data\DS_TMH\work in progress 1.3\FMG test files\Items - English - vanilla\Item_name_.fmg";
            //Tbx_Cmp_iFile2.Text = @"C:\Sandbox\Modding data\DS_TMH\work in progress 1.3\FMG test files\Items - English - Daughters of Ash\ItemNames.fmg";

            //Tbx_Prp_iFile1.Text = @"C:\Sandbox\Modding data\DS_TMH\published 1.2\FMG test files\English - Daughters of Ash\RingDescriptions.fmg";
            //Tbx_Prp_iFile2.Text = @"C:\Sandbox\Modding data\DS_TMH\published 1.2\FMG test files\English - vanilla\Accessory_long_desc_.fmg";
            //Tbx_Prp_iFile3.Text = @"C:\Sandbox\Modding data\DS_TMH\published 1.2\FMG test files\French - vanilla\Accessory_long_desc_.fmg";



            //Tbx_Cmp_iFile1.Text = @"C:\Sandbox\Modding data\DS_TMH\published 1.2\FMG test files\English - Daughters of Ash\RingDescriptions.fmg";
            //Tbx_Cmp_iFile2.Text = @"C:\Sandbox\Modding data\DS_TMH\published 1.2\FMG test files\English - vanilla\Accessory_long_desc_.fmg";

            //Tbx_Prp_iFile1.Text = @"C:\Sandbox\Modding data\DS_TMH\work in progress 1.3\FMG test files\Items - English - Daughters of Ash\ArmorDescriptions.fmg";
            //Tbx_Prp_iFile2.Text = @"C:\Sandbox\Modding data\DS_TMH\work in progress 1.3\FMG test files\Items - English - vanilla\Armor_long_desc_.fmg";
            //Tbx_Prp_iFile3.Text = @"C:\Sandbox\Modding data\DS_TMH\work in progress 1.3\FMG test files\Items - Italian - vanilla\Armor_long_desc_.fmg";
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
        private void Btn_Cmp_ExploreFile1_Click(object sender, RoutedEventArgs e)
        {
            Explore(sender);
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
        private void Btn_Cmp_ExploreFile2_Click(object sender, RoutedEventArgs e)
        {
            Explore(sender);
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
        private void Btn_Prp_ExploreFile1_Click(object sender, RoutedEventArgs e)
        {
            Explore(sender);
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
        private void Btn_Prp_ExploreFile2_Click(object sender, RoutedEventArgs e)
        {
            Explore(sender);
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
        private void Btn_Prp_ExploreFile3_Click(object sender, RoutedEventArgs e)
        {
            Explore(sender);
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

        private void Explore(object sender)
        {
            Button btn = sender as Button;

            switch (btn.Name)
            {
                case "Btn_Cmp_ExploreFile1":
                    _ = Process.Start(Tbx_Cmp_iFile1.Text != TBX_DEFAULT ? IOHelper.GetParentFolder(Tbx_Cmp_iFile1.Text) : IOHelper.GetRootDirPath());
                    break;
                case "Btn_Cmp_ExploreFile2":
                    _ = Process.Start(Tbx_Cmp_iFile2.Text != TBX_DEFAULT ? IOHelper.GetParentFolder(Tbx_Cmp_iFile2.Text) : IOHelper.GetRootDirPath());
                    break;
                case "Btn_Prp_ExploreFile1":
                    _ = Process.Start(Tbx_Prp_iFile1.Text != TBX_DEFAULT ? IOHelper.GetParentFolder(Tbx_Prp_iFile1.Text) : IOHelper.GetRootDirPath());
                    break;
                case "Btn_Prp_ExploreFile2":
                    _ = Process.Start(Tbx_Prp_iFile2.Text != TBX_DEFAULT ? IOHelper.GetParentFolder(Tbx_Prp_iFile2.Text) : IOHelper.GetRootDirPath());
                    break;
                case "Btn_Prp_ExploreFile3":
                    _ = Process.Start(Tbx_Prp_iFile3.Text != TBX_DEFAULT ? IOHelper.GetParentFolder(Tbx_Prp_iFile3.Text) : IOHelper.GetRootDirPath());
                    break;
                default:
                    _ = Process.Start(IOHelper.GetRootDirPath());
                    break;
            }
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
                        Tbx_Cmp_oFilename.Text = IOHelper.GetFilenameFromPath(Tbx_Cmp_iFile1.Text);
                    else if (Cmbx_Cmp_TargetInputFilename.SelectedIndex == 1 && tbx == Tbx_Cmp_iFile2)
                        Tbx_Cmp_oFilename.Text = IOHelper.GetFilenameFromPath(Tbx_Cmp_iFile2.Text);

                    ValidateTbxValue(Tbx_Cmp_oFilename);
                }
                if (tbx.Name.Contains("Prp") && (Cbx_Prp_UseInputFilename.IsChecked ?? true)) // Sender is TextBox from Prepare mode
                {
                    if (Cmbx_Prp_TargetInputFilename.SelectedIndex == 0 && tbx == Tbx_Prp_iFile1)
                        Tbx_Prp_oFilename.Text = IOHelper.GetFilenameFromPath(Tbx_Prp_iFile1.Text);
                    else if (Cmbx_Prp_TargetInputFilename.SelectedIndex == 1 && tbx == Tbx_Prp_iFile2)
                        Tbx_Prp_oFilename.Text = IOHelper.GetFilenameFromPath(Tbx_Prp_iFile2.Text);
                    else if (Cmbx_Prp_TargetInputFilename.SelectedIndex == 2 && tbx == Tbx_Prp_iFile3)
                        Tbx_Prp_oFilename.Text = IOHelper.GetFilenameFromPath(Tbx_Prp_iFile3.Text);

                    ValidateTbxValue(Tbx_Prp_oFilename);
                }
            }

            if (sender is ComboBox)
            {
                ComboBox cmbx = sender as ComboBox;

                if (cmbx == Cmbx_Cmp_TargetInputFilename) // Sender is ComboBox from Compare mode
                {
                    if (cmbx.SelectedIndex == 0 && Tbx_Cmp_iFile1.Text != TBX_DEFAULT)
                        Tbx_Cmp_oFilename.Text = IOHelper.GetFilenameFromPath(Tbx_Cmp_iFile1.Text);
                    else if (cmbx.SelectedIndex == 1 && Tbx_Cmp_iFile2.Text != TBX_DEFAULT)
                        Tbx_Cmp_oFilename.Text = IOHelper.GetFilenameFromPath(Tbx_Cmp_iFile2.Text);

                    ValidateTbxValue(Tbx_Cmp_oFilename);
                }
                if (cmbx == Cmbx_Prp_TargetInputFilename) // Sender is ComboBox from Prepare mode
                {
                    if (cmbx.SelectedIndex == 0 && Tbx_Prp_iFile1.Text != TBX_DEFAULT)
                        Tbx_Prp_oFilename.Text = IOHelper.GetFilenameFromPath(Tbx_Prp_iFile1.Text);
                    else if (cmbx.SelectedIndex == 1 && Tbx_Prp_iFile2.Text != TBX_DEFAULT)
                        Tbx_Prp_oFilename.Text = IOHelper.GetFilenameFromPath(Tbx_Prp_iFile2.Text);
                    else if (cmbx.SelectedIndex == 2 && Tbx_Prp_iFile3.Text != TBX_DEFAULT)
                        Tbx_Prp_oFilename.Text = IOHelper.GetFilenameFromPath(Tbx_Prp_iFile3.Text);

                    ValidateTbxValue(Tbx_Prp_oFilename);
                }
            }
        }

        #endregion


        #region Preview

        //TODO: Find better display condition than Visibility property

        private void Btn_RefreshPreview_Click(object sender, RoutedEventArgs e)
        {
            bool allDetails = Cbx_PreviewAllDetails.IsChecked ?? false;

            if (Tbc_Modes.SelectedIndex == 0)
            {
                string iFile1 = Tbx_Cmp_iFile1.Text;
                string iFile2 = Tbx_Cmp_iFile2.Text;

                if (iFile1 == "" || iFile1 == TBX_DEFAULT || iFile2 == "" || iFile2 == TBX_DEFAULT)
                {
                    _ = MessageBox.Show(ERR_MISSING_IFILES);
                    return;
                }
                Dtg_Preview.Visibility = Visibility.Visible;
                Dtg_Preview.Columns.Clear();

                CompareMode c = new CompareMode(iFile1, iFile2) { OneLinedValues = Cbx_Cmp_OneLinedValues.IsChecked ?? false };
                c.ProcessFiles(true);

                foreach (DataGridTextColumn col in GetCompareColumns(allDetails, false))
                    Dtg_Preview.Columns.Add(col);

                Dtg_Preview.ItemsSource = c.Entries;
            }
            else
            {
                string iFile1 = Tbx_Prp_iFile1.Text;
                string iFile2 = Tbx_Prp_iFile2.Text;
                string iFile3 = Tbx_Prp_iFile3.Text;
                string textToReplace = Tbx_Prp_TextToReplace.Text;
                string replacingText = Tbx_Prp_ReplacingText.Text;

                if (iFile1 == "" || iFile1 == TBX_DEFAULT || iFile2 == "" || iFile2 == TBX_DEFAULT || iFile3 == "" || iFile3 == TBX_DEFAULT)
                {
                    _ = MessageBox.Show(ERR_MISSING_IFILES);
                    return;
                }
                Dtg_Preview.Visibility = Visibility.Visible;
                Dtg_Preview.Columns.Clear();

                PrepareMode p = new PrepareMode(iFile1, iFile2, iFile3, textToReplace, replacingText);
                p.ProcessFiles(true);

                foreach (DataGridTextColumn col in GetPrepareColumns(allDetails, false))
                    Dtg_Preview.Columns.Add(col);

                Dtg_Preview.ItemsSource = p.Entries;
            }

        }

        private void Btn_ClearPreview_Click(object sender, RoutedEventArgs e)
        {
            if (Dtg_Preview.Visibility == Visibility.Visible && Dtg_Preview.ItemsSource != null)
            {
                Dtg_Preview.Visibility = Visibility.Hidden;
                Dtg_Preview.ItemsSource = null;
            }
        }

        private void Btn_DetachPreview_Click(object sender, RoutedEventArgs e) // TODO! Improve by using XAML
        {
            if (Dtg_Preview.Visibility != Visibility.Visible)
                return;

            //DataContext = Dtg_Preview.ItemsSource;

            DataGrid detachedDtgPreview = new DataGrid()
            {
                Margin = new Thickness(16, 16, 16, 48), // Extra bottom margin = TODO? Replicate lower buttons

                AutoGenerateColumns = false,
                HeadersVisibility = DataGridHeadersVisibility.Column,
                IsReadOnly = false,
                ItemsSource = Dtg_Preview.ItemsSource
            };

            bool allDetails = Cbx_PreviewAllDetails.IsChecked ?? false;
            List<DataGridTextColumn> columns = Tbc_Modes.SelectedIndex == 0 ? GetCompareColumns(allDetails, true) : GetPrepareColumns(allDetails, true);

            foreach (DataGridTextColumn col in columns)
                detachedDtgPreview.Columns.Add(col);

            Window windowPreview = new Window()
            {
                Title = "Output preview (detached)",
                Width = 1280,
                Height = 800,
                Background = (Brush)new BrushConverter().ConvertFrom("#EEE"),

                Content = detachedDtgPreview,
            };

            _ = windowPreview.ShowDialog();
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

        private List<DataGridTextColumn> GetCompareColumns(bool allDetails, bool detached) // TODO! Something easier
        {
            List<DataGridTextColumn> columns = new List<DataGridTextColumn>();

            double COL_MAXWIDTH = IOHelper.GetColumnMaxWidth() * 2; // 1080 or 720 depending of user's screen resolution

            double maxWidth_RowNum = detached ? 80 : 40;
            double maxWidth_TextId = detached ? 160 : 80;
            double maxWidth_Value1 = detached ? COL_MAXWIDTH : 480;
            double maxWidth_Value2 = detached ? COL_MAXWIDTH : 480;

            string oHeader1 = Tbx_Cmp_oHeader1.Text != "" ? Tbx_Cmp_oHeader1.Text : "Header #1";
            string oHeader2 = Tbx_Cmp_oHeader2.Text != "" ? Tbx_Cmp_oHeader2.Text : "Header #2";

            columns.Add(new DataGridTextColumn() { Header = "Text ID", Binding = new Binding("TextId"), MaxWidth = maxWidth_TextId });
            columns.Add(new DataGridTextColumn() { Header = oHeader1, Binding = new Binding("Value1"), MaxWidth = maxWidth_Value1 });
            columns.Add(new DataGridTextColumn() { Header = oHeader2, Binding = new Binding("Value2"), MaxWidth = maxWidth_Value2 });
            columns.Add(new DataGridTextColumn() { Header = "Same?", Binding = new Binding("Same"), MaxWidth = 80 });

            if (allDetails)
            {
                Style hdrOff = (Style)Grid_Main.Resources["HeaderOff"]; //Style style = new Style() // TODO? See how to do this from code behind
                columns.Insert(0, new DataGridTextColumn() { Header = "#", HeaderStyle = hdrOff, Foreground = Brushes.Gray, Binding = new Binding("Index"), MaxWidth = maxWidth_RowNum });
            }

            return columns;
        }

        private List<DataGridTextColumn> GetPrepareColumns(bool allDetails, bool detached) // TODO! Something easier
        {
            List<DataGridTextColumn> columns = new List<DataGridTextColumn>();

            double COL_MAXWIDTH = IOHelper.GetColumnMaxWidth(); // 540 or 360 depending of user's screen resolution

            double maxWidth_RowNum = detached ? 80 : 40;
            double maxWidth_TextId = detached ? 200 : allDetails ? 80 : 120;
            double maxWidth_Output = detached ? COL_MAXWIDTH : allDetails ? 240 : 360;
            double maxWidth_Value1 = detached ? COL_MAXWIDTH : 240;
            double maxWidth_Value2 = detached ? COL_MAXWIDTH : 240;
            double maxWidth_Value3 = detached ? COL_MAXWIDTH : 240;

            columns.Add(new DataGridTextColumn() { Header = "Text ID", Binding = new Binding("TextId"), MaxWidth = maxWidth_TextId });
            columns.Add(new DataGridTextColumn() { Header = "Output value", Binding = new Binding("Output"), MaxWidth = maxWidth_Output });

            if (allDetails)
            {
                Style hdrOff = (Style)Grid_Main.Resources["HeaderOff"];
                columns.Insert(0, new DataGridTextColumn() { Header = "#", HeaderStyle = hdrOff, Foreground = Brushes.Gray, Binding = new Binding("Index"), MaxWidth = maxWidth_RowNum });
                columns.Insert(2, new DataGridTextColumn() { Header = "File #1 value", HeaderStyle = hdrOff, Foreground = Brushes.Gray, Binding = new Binding("Value1"), MaxWidth = maxWidth_Value1 });
                columns.Insert(3, new DataGridTextColumn() { Header = "File #2 value", HeaderStyle = hdrOff, Foreground = Brushes.Gray, Binding = new Binding("Value2"), MaxWidth = maxWidth_Value2 });
                columns.Insert(4, new DataGridTextColumn() { Header = "File #3 value", HeaderStyle = hdrOff, Foreground = Brushes.Gray, Binding = new Binding("Value3"), MaxWidth = maxWidth_Value3 });
                columns.Insert(6, new DataGridTextColumn() { Header = "From?", HeaderStyle = hdrOff, Foreground = Brushes.Gray, Binding = new Binding("Source"), MaxWidth = 80 });
            }

            return columns;
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
                errors.Add(ERR_MISSING_IFILES);

            if (input_filepath1 != TBX_DEFAULT && input_filepath1 == input_filepath2)
                errors.Add(ERR_SAME_IFILE);

            if (output_header_1 == "" || output_header_2 == "")
                errors.Add(ERR_MISSING_OHDRS);

            if (output_filename == "")
                errors.Add(ERR_MISSING_OFNAME);

            if (sepa_csv_char_o == "")
                errors.Add(ERR_MISSING_CSVSEP);

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
                errors.Add(ERR_MISSING_IFILES);

            if ((input_filepath1 != TBX_DEFAULT && input_filepath1 == input_filepath2) ||
                (input_filepath2 != TBX_DEFAULT && input_filepath2 == input_filepath3) ||
                (input_filepath3 != TBX_DEFAULT && input_filepath3 == input_filepath1))
                errors.Add(ERR_SAME_IFILE);

            if (output_filename == "")
                errors.Add(ERR_MISSING_OFNAME);

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
