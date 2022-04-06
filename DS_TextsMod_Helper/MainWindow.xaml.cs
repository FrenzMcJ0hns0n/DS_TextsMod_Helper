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


        #region ENUM

        private enum PROCESS_MODE : int
        {
            None = -1,
            Read = 0,
            Compare = 1,
            Prepare = 2,
        };

        /// <summary>
        /// Return PROCESS MODE currently active in main TabControl
        /// </summary>
        private PROCESS_MODE SelectedMode()
        {
            switch (Tbc_Modes.SelectedIndex)
            {
                case 0: return PROCESS_MODE.Read;
                case 1: return PROCESS_MODE.Compare;
                case 2: return PROCESS_MODE.Prepare;
                default: return PROCESS_MODE.None;
            }
        }

        /// <summary>
        /// Return PROCESS MODE currently loaded in Preview DataGrid
        /// </summary>
        private PROCESS_MODE LoadedMode()
        {
            if (Dtg_Preview.ItemsSource is List<ReadMode.Entry>)
                return PROCESS_MODE.Read;

            if (Dtg_Preview.ItemsSource is List<CompareMode.Entry>)
                return PROCESS_MODE.Compare;

            if (Dtg_Preview.ItemsSource is List<PrepareMode.Entry>)
                return PROCESS_MODE.Prepare;

            return PROCESS_MODE.None;
        }


        #endregion


        public MainWindow()
        {
            InitializeComponent();

            Directory.CreateDirectory(Tools.GetOutputDirPath());

            if (!File.Exists(Path.Combine(Tools.GetRootDirPath(), "SoulsFormats.dll")))
                MessageBox.Show("Fatal error : file 'SoulsFormats.dll' not found");

#if DEBUG
            // TODO
#endif
        }


        #region GUI Read mode

        private void Tbk_Rd_iFile1_PreviewDragOver(object sender, DragEventArgs e) { e.Handled = true; }
        private void Tbk_Rd_iFile1_Drop(object sender, DragEventArgs e)
        {
            if (AcceptDroppedInputFile(e))
            {
                DisplayInputFilepath(sender, e);
                SyncFilenames(sender);
            }
        }
        private void Btn_Rd_ExploreFile1_Click(object sender, RoutedEventArgs e) { Explore(sender); }
        private void Tbx_Rd_oFilename_GotFocus(object sender, RoutedEventArgs e) { SelectTbxValue(sender); }
        private void Tbx_Rd_oFilename_LostFocus(object sender, RoutedEventArgs e) { ValidateTbxValue(sender); }
        private void Cbx_Rd_UseInputFilename_Checked(object sender, RoutedEventArgs e) { SyncFilenames(sender); }
        private void Tbx_Rd_CsvSeparator_GotFocus(object sender, RoutedEventArgs e) { SelectTbxValue(sender); }
        private void Tbx_Rd_CsvSeparator_LostFocus(object sender, RoutedEventArgs e) { ValidateTbxValue(sender); }

        #endregion


        #region GUI Compare mode

        private void Tbk_Cmp_iFile1_PreviewDragOver(object sender, DragEventArgs e) { e.Handled = true; }
        private void Tbk_Cmp_iFile1_Drop(object sender, DragEventArgs e)
        {
            if (AcceptDroppedInputFile(e))
            {
                DisplayInputFilepath(sender, e);
                SyncFilenames(sender);
            }
        }
        private void Btn_Cmp_ExploreFile1_Click(object sender, RoutedEventArgs e) { Explore(sender); }
        private void Tbk_Cmp_iFile2_PreviewDragOver(object sender, DragEventArgs e) { e.Handled = true; }
        private void Tbk_Cmp_iFile2_Drop(object sender, DragEventArgs e)
        {
            if (AcceptDroppedInputFile(e))
            {
                DisplayInputFilepath(sender, e);
                SyncFilenames(sender);
            }
        }
        private void Btn_Cmp_ExploreFile2_Click(object sender, RoutedEventArgs e) { Explore(sender); }
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

        private void Tbk_Prp_iFile1_PreviewDragOver(object sender, DragEventArgs e) { e.Handled = true; }
        private void Tbk_Prp_iFile1_Drop(object sender, DragEventArgs e)
        {
            if (AcceptDroppedInputFile(e))
            {
                DisplayInputFilepath(sender, e);
                SyncFilenames(sender);
            }
        }
        private void Btn_Prp_ExploreFile1_Click(object sender, RoutedEventArgs e) { Explore(sender); }
        private void Tbk_Prp_iFile2_PreviewDragOver(object sender, DragEventArgs e) { e.Handled = true; }
        private void Tbk_Prp_iFile2_Drop(object sender, DragEventArgs e)
        {
            if (AcceptDroppedInputFile(e))
            {
                DisplayInputFilepath(sender, e);
                SyncFilenames(sender);
            }
        }
        private void Btn_Prp_ExploreFile2_Click(object sender, RoutedEventArgs e) { Explore(sender); }
        private void Tbk_Prp_iFile3_PreviewDragOver(object sender, DragEventArgs e) { e.Handled = true; }
        private void Tbk_Prp_iFile3_Drop(object sender, DragEventArgs e)
        {
            if (AcceptDroppedInputFile(e))
            {
                DisplayInputFilepath(sender, e);
                SyncFilenames(sender);
            }
        }
        private void Btn_Prp_ExploreFile3_Click(object sender, RoutedEventArgs e) { Explore(sender); }
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
                case "Btn_Rd_ExploreFile1":
                    _ = Process.Start(Tbk_Rd_iFile1.Text != TBX_DEFAULT ? Tools.GetParentFolder(Tbk_Rd_iFile1.Text) : Tools.GetRootDirPath());
                    break;
                case "Btn_Cmp_ExploreFile1":
                    _ = Process.Start(Tbk_Cmp_iFile1.Text != TBX_DEFAULT ? Tools.GetParentFolder(Tbk_Cmp_iFile1.Text) : Tools.GetRootDirPath());
                    break;
                case "Btn_Cmp_ExploreFile2":
                    _ = Process.Start(Tbk_Cmp_iFile2.Text != TBX_DEFAULT ? Tools.GetParentFolder(Tbk_Cmp_iFile2.Text) : Tools.GetRootDirPath());
                    break;
                case "Btn_Prp_ExploreFile1":
                    _ = Process.Start(Tbk_Prp_iFile1.Text != TBX_DEFAULT ? Tools.GetParentFolder(Tbk_Prp_iFile1.Text) : Tools.GetRootDirPath());
                    break;
                case "Btn_Prp_ExploreFile2":
                    _ = Process.Start(Tbk_Prp_iFile2.Text != TBX_DEFAULT ? Tools.GetParentFolder(Tbk_Prp_iFile2.Text) : Tools.GetRootDirPath());
                    break;
                case "Btn_Prp_ExploreFile3":
                    _ = Process.Start(Tbk_Prp_iFile3.Text != TBX_DEFAULT ? Tools.GetParentFolder(Tbk_Prp_iFile3.Text) : Tools.GetRootDirPath());
                    break;
                default:
                    _ = Process.Start(Tools.GetRootDirPath());
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

        private void SyncFilenames(object sender) // TODO: Factorize (use PROCESS_MODE ?)
        {
            if (sender is CheckBox && Tbk_Rd_iFile1.Text != TBX_DEFAULT) // Sender is CheckBox from Read mode
            {
                Tbx_Rd_oFilename.Text = Tools.GetFilenameFromPath(Tbk_Rd_iFile1.Text);
                ValidateTbxValue(Tbx_Rd_oFilename);
            }

            if (sender is TextBlock)
            {
                TextBlock tbk = sender as TextBlock;

                if (tbk.Name == "Tbx_Rd_iFile1" && (Cbx_Rd_UseInputFilename.IsChecked ?? true)) // Sender is TextBox from Read mode
                {
                    Tbx_Rd_oFilename.Text = Tools.GetFilenameFromPath(Tbk_Rd_iFile1.Text);
                    ValidateTbxValue(Tbx_Rd_oFilename);
                }
                if (tbk.Name.Contains("Cmp") && (Cbx_Cmp_UseInputFilename.IsChecked ?? true)) // Sender is TextBox from Compare mode
                {
                    if (Cmbx_Cmp_TargetInputFilename.SelectedIndex == 0 && tbk == Tbk_Cmp_iFile1)
                        Tbx_Cmp_oFilename.Text = Tools.GetFilenameFromPath(Tbk_Cmp_iFile1.Text);
                    else if (Cmbx_Cmp_TargetInputFilename.SelectedIndex == 1 && tbk == Tbk_Cmp_iFile2)
                        Tbx_Cmp_oFilename.Text = Tools.GetFilenameFromPath(Tbk_Cmp_iFile2.Text);

                    ValidateTbxValue(Tbx_Cmp_oFilename);
                }
                if (tbk.Name.Contains("Prp") && (Cbx_Prp_UseInputFilename.IsChecked ?? true)) // Sender is TextBox from Prepare mode
                {
                    if (Cmbx_Prp_TargetInputFilename.SelectedIndex == 0 && tbk == Tbk_Prp_iFile1)
                        Tbx_Prp_oFilename.Text = Tools.GetFilenameFromPath(Tbk_Prp_iFile1.Text);
                    else if (Cmbx_Prp_TargetInputFilename.SelectedIndex == 1 && tbk == Tbk_Prp_iFile2)
                        Tbx_Prp_oFilename.Text = Tools.GetFilenameFromPath(Tbk_Prp_iFile2.Text);
                    else if (Cmbx_Prp_TargetInputFilename.SelectedIndex == 2 && tbk == Tbk_Prp_iFile3)
                        Tbx_Prp_oFilename.Text = Tools.GetFilenameFromPath(Tbk_Prp_iFile3.Text);

                    ValidateTbxValue(Tbx_Prp_oFilename);
                }
            }

            if (sender is ComboBox)
            {
                ComboBox cmbx = sender as ComboBox;

                if (cmbx == Cmbx_Cmp_TargetInputFilename) // Sender is ComboBox from Compare mode
                {
                    if (cmbx.SelectedIndex == 0 && Tbk_Cmp_iFile1.Text != TBX_DEFAULT)
                        Tbx_Cmp_oFilename.Text = Tools.GetFilenameFromPath(Tbk_Cmp_iFile1.Text);
                    else if (cmbx.SelectedIndex == 1 && Tbk_Cmp_iFile2.Text != TBX_DEFAULT)
                        Tbx_Cmp_oFilename.Text = Tools.GetFilenameFromPath(Tbk_Cmp_iFile2.Text);

                    ValidateTbxValue(Tbx_Cmp_oFilename);
                }
                if (cmbx == Cmbx_Prp_TargetInputFilename) // Sender is ComboBox from Prepare mode
                {
                    if (cmbx.SelectedIndex == 0 && Tbk_Prp_iFile1.Text != TBX_DEFAULT)
                        Tbx_Prp_oFilename.Text = Tools.GetFilenameFromPath(Tbk_Prp_iFile1.Text);
                    else if (cmbx.SelectedIndex == 1 && Tbk_Prp_iFile2.Text != TBX_DEFAULT)
                        Tbx_Prp_oFilename.Text = Tools.GetFilenameFromPath(Tbk_Prp_iFile2.Text);
                    else if (cmbx.SelectedIndex == 2 && Tbk_Prp_iFile3.Text != TBX_DEFAULT)
                        Tbx_Prp_oFilename.Text = Tools.GetFilenameFromPath(Tbk_Prp_iFile3.Text);

                    ValidateTbxValue(Tbx_Prp_oFilename);
                }
            }
        }

        #endregion


        #region Preview

        private void Btn_RefreshPreview_Click(object sender, RoutedEventArgs e)
        {
            switch (SelectedMode())
            {
                case PROCESS_MODE.Read: PreviewRead(); break;
                case PROCESS_MODE.Compare: PreviewCompare(); break;
                case PROCESS_MODE.Prepare: PreviewPrepare(); break;
            }
        }

        private void Btn_ClearPreview_Click(object sender, RoutedEventArgs e)
        {
            if (LoadedMode() != PROCESS_MODE.None)
            {
                Dtg_Preview.Visibility = Visibility.Hidden;
                Dtg_Preview.ItemsSource = null;
            }
        }

        private void Btn_DetachPreview_Click(object sender, RoutedEventArgs e) // TODO! Improve by using XAML
        {
            if (LoadedMode() == PROCESS_MODE.None)
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
            bool detached = true;

            List<DataGridTextColumn> columns = null;
            string processMode = null;

            switch (LoadedMode())
            {
                case PROCESS_MODE.Read: columns = GetReadColumns(allDetails, detached); processMode = "Read mode"; break;
                case PROCESS_MODE.Compare: columns = GetCompareColumns(allDetails, detached); processMode = "Compare mode"; break;
                case PROCESS_MODE.Prepare: columns = GetPrepareColumns(allDetails, detached); processMode = "Prepare mode"; break;
            }

            foreach (DataGridTextColumn col in columns)
                detachedDtgPreview.Columns.Add(col);

            Window windowPreview = new Window()
            {
                Title = "Detached output preview : " + processMode,
                Width = 1280,
                Height = 800,
                Background = (Brush)new BrushConverter().ConvertFrom("#EEE"),

                Content = detachedDtgPreview,
            };

            _ = windowPreview.ShowDialog();
        }

        private void Cbx_PreviewAllDetails_Checked(object sender, RoutedEventArgs e)
        {
            switch (LoadedMode())
            {
                case PROCESS_MODE.None: return;
                case PROCESS_MODE.Read: PreviewRead((List<ReadMode.Entry>)Dtg_Preview.ItemsSource); break;
                case PROCESS_MODE.Compare: PreviewCompare((List<CompareMode.Entry>)Dtg_Preview.ItemsSource); break;
                case PROCESS_MODE.Prepare: PreviewPrepare((List<PrepareMode.Entry>)Dtg_Preview.ItemsSource); break;
            }
        }

        private void Cbx_PreviewAllDetails_Unchecked(object sender, RoutedEventArgs e)
        {
            switch (LoadedMode())
            {
                case PROCESS_MODE.None: return;
                case PROCESS_MODE.Read: PreviewRead((List<ReadMode.Entry>)Dtg_Preview.ItemsSource); break;
                case PROCESS_MODE.Compare: PreviewCompare((List<CompareMode.Entry>)Dtg_Preview.ItemsSource); break;
                case PROCESS_MODE.Prepare: PreviewPrepare((List<PrepareMode.Entry>)Dtg_Preview.ItemsSource); break;
            }
        }

        private void PreviewRead(List<ReadMode.Entry> r_entries = null)
        {
            if (r_entries is null)
            {
                string iFile1 = Tbk_Rd_iFile1.Text;

                if (iFile1 == "" || iFile1 == TBX_DEFAULT)
                {
                    _ = MessageBox.Show("[Read mode] " + ERR_MISSING_IFILES);
                    return;
                }

                ReadMode r = new ReadMode(iFile1) { OneLinedValues = Cbx_Rd_OneLinedValues.IsChecked ?? false };
                r.ProcessFiles(true);
                r_entries = r.Entries;
            }

            Dtg_Preview.Visibility = Visibility.Visible;
            Dtg_Preview.Columns.Clear();

            bool allDetails = Cbx_PreviewAllDetails.IsChecked ?? false;
            bool detached = false;

            List<DataGridTextColumn> columns = GetReadColumns(allDetails, detached);
            foreach (DataGridTextColumn col in columns)
                Dtg_Preview.Columns.Add(col);

            Dtg_Preview.ItemsSource = r_entries;

        }

        private void PreviewCompare(List<CompareMode.Entry> c_entries = null)
        {
            if (c_entries is null)
            {
                string iFile1 = Tbk_Cmp_iFile1.Text;
                string iFile2 = Tbk_Cmp_iFile2.Text;

                if (iFile1 == "" || iFile1 == TBX_DEFAULT || iFile2 == "" || iFile2 == TBX_DEFAULT)
                {
                    _ = MessageBox.Show("[Compare mode] " + ERR_MISSING_IFILES);
                    return;
                }

                CompareMode c = new CompareMode(iFile1, iFile2) { OneLinedValues = Cbx_Cmp_OneLinedValues.IsChecked ?? false };
                c.ProcessFiles(true);
                c_entries = c.Entries;
            }

            Dtg_Preview.Visibility = Visibility.Visible;
            Dtg_Preview.Columns.Clear();

            bool allDetails = Cbx_PreviewAllDetails.IsChecked ?? false;
            bool detached = false;

            List<DataGridTextColumn> columns = GetCompareColumns(allDetails, detached);
            foreach (DataGridTextColumn col in columns)
                Dtg_Preview.Columns.Add(col);

            Dtg_Preview.ItemsSource = c_entries;
        }

        private void PreviewPrepare(List<PrepareMode.Entry> p_entries = null)
        {
            if (p_entries is null)
            {
                string iFile1 = Tbk_Prp_iFile1.Text;
                string iFile2 = Tbk_Prp_iFile2.Text;
                string iFile3 = Tbk_Prp_iFile3.Text;
                string textToReplace = Tbx_Prp_TextToReplace.Text;
                string replacingText = Tbx_Prp_ReplacingText.Text;

                if (iFile1 == "" || iFile1 == TBX_DEFAULT || iFile2 == "" || iFile2 == TBX_DEFAULT || iFile3 == "" || iFile3 == TBX_DEFAULT)
                {
                    _ = MessageBox.Show("[Prepare mode] " + ERR_MISSING_IFILES);
                    Cbx_PreviewAllDetails.IsChecked = false;
                    return;
                }

                PrepareMode p = new PrepareMode(iFile1, iFile2, iFile3, textToReplace, replacingText);
                p.ProcessFiles(true);
                p_entries = p.Entries;
            }

            Dtg_Preview.Visibility = Visibility.Visible;
            Dtg_Preview.Columns.Clear();

            bool allDetails = Cbx_PreviewAllDetails.IsChecked ?? false;
            bool detached = false;

            List<DataGridTextColumn> columns = GetPrepareColumns(allDetails, detached);
            foreach (DataGridTextColumn col in columns)
                Dtg_Preview.Columns.Add(col);

            Dtg_Preview.ItemsSource = p_entries;
        }

        private List<DataGridTextColumn> GetReadColumns(bool allDetails, bool detached)
        {
            List<DataGridTextColumn> columns = new List<DataGridTextColumn>();

            double COL_MAXWIDTH = Tools.GetColumnMaxWidth() * 4; // 2160 or 1440 depending of user's screen resolution

            double maxWidth_RowNum = detached ? 80 : 40;
            double maxWidth_TextId = detached ? 120 : 80;
            double maxWidth_Value = detached ? COL_MAXWIDTH : 1000;

            columns.Add(new DataGridTextColumn() { Header = "Text ID", Binding = new Binding("TextId"), MaxWidth = maxWidth_TextId });
            columns.Add(new DataGridTextColumn() { Header = "Value", Binding = new Binding("Value"), MaxWidth = maxWidth_Value });

            if (allDetails)
            {
                Style hdrOff = (Style)Grid_Main.Resources["HeaderOff"]; //Style style = new Style() // TODO? See how to do this from code behind
                columns.Insert(0, new DataGridTextColumn() { Header = "#", HeaderStyle = hdrOff, Foreground = Brushes.Gray, Binding = new Binding("Index"), MaxWidth = maxWidth_RowNum });
            }

            return columns;
        }

        private List<DataGridTextColumn> GetCompareColumns(bool allDetails, bool detached) // TODO! Something easier
        {
            List<DataGridTextColumn> columns = new List<DataGridTextColumn>();

            double COL_MAXWIDTH = Tools.GetColumnMaxWidth() * 2; // 1080 or 720 depending of user's screen resolution

            double maxWidth_RowNum = detached ? 80 : 40;
            double maxWidth_TextId = detached ? 120 : 80;
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

            double COL_MAXWIDTH = Tools.GetColumnMaxWidth(); // 540 or 360 depending of user's screen resolution

            double maxWidth_RowNum = detached ? 80 : 40;
            double maxWidth_TextId = detached ? 120 : allDetails ? 80 : 120;
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
            switch (SelectedMode())
            {
                case PROCESS_MODE.Read:

                    if (!ValidateReadInputs())
                        return;

                    string rd_iFile1 = Tbk_Rd_iFile1.Text;
                    string rd_oFilename = Tbx_Rd_oFilename.Text + ".csv";
                    string rd_csvSepChar = Tbx_Rd_CsvSeparator.Text;

                    ReadMode r = new ReadMode(rd_iFile1) { OneLinedValues = Cbx_Rd_OneLinedValues.IsChecked ?? false };
                    r.ProcessFiles(false);
                    r.ProduceOutput(rd_oFilename, rd_csvSepChar); // TODO : Add csvSepChar as var

                    _ = MessageBox.Show($"[Read mode] File \"{r.OutputFilename}\" created");
                    break;

                case PROCESS_MODE.Compare:

                    if (!ValidateCompareInputs())
                        return;

                    string cmp_iFile1 = Tbk_Cmp_iFile1.Text;
                    string cmp_iFile2 = Tbk_Cmp_iFile2.Text;
                    string cmp_oFilename = Tbx_Cmp_oFilename.Text + ".csv";
                    string oHdr1 = Tbx_Cmp_oHeader1.Text;
                    string oHdr2 = Tbx_Cmp_oHeader2.Text;
                    string cmp_csvSepChar = Tbx_Cmp_CsvSeparator.Text;

                    CompareMode c = new CompareMode(cmp_iFile1, cmp_iFile2) { OneLinedValues = Cbx_Cmp_OneLinedValues.IsChecked ?? false };
                    c.ProcessFiles(false);
                    c.ProduceOutput(cmp_oFilename, oHdr1, oHdr2, cmp_csvSepChar);

                    _ = MessageBox.Show($"[Compare mode] File \"{c.OutputFilename}\" created");
                    break;

                case PROCESS_MODE.Prepare:

                    if (!ValidatePrepareInputs())
                        return;

                    string prp_iFile1 = Tbk_Prp_iFile1.Text;
                    string prp_iFile2 = Tbk_Prp_iFile2.Text;
                    string prp_iFile3 = Tbk_Prp_iFile3.Text;
                    string prp_oFilename = Tbx_Prp_oFilename.Text + ".fmg";
                    string textToReplace = Tbx_Prp_TextToReplace.Text;
                    string replacingText = Tbx_Prp_ReplacingText.Text;

                    PrepareMode p = new PrepareMode(prp_iFile1, prp_iFile2, prp_iFile3, textToReplace, replacingText);
                    p.ProcessFiles(false);
                    p.ProduceOutput(prp_oFilename);

                    _ = MessageBox.Show($"[Prepare mode] File \"{p.OutputFilename}\" created");
                    break;
            }
        }

        private bool ValidateReadInputs()
        {
            string mode = "[Read mode] ";
            List<string> errors = new List<string>();

            string input_filepath1 = Tbk_Rd_iFile1.Text;
            string output_filename = Tbx_Rd_oFilename.Text;
            string sepa_csv_char_o = Tbx_Rd_CsvSeparator.Text;

            if (!File.Exists(input_filepath1))
                errors.Add(mode + ERR_MISSING_IFILES);

            if (output_filename == "")
                errors.Add(mode + ERR_MISSING_OFNAME);

            if (sepa_csv_char_o == "")
                errors.Add(mode + ERR_MISSING_CSVSEP);

            if (errors.Count > 0)
            {
                _ = MessageBox.Show(string.Join("\n\n", errors));
                return false;
            }
            return true;
        }

        private bool ValidateCompareInputs()
        {
            string mode = "[Compare mode] ";
            List<string> errors = new List<string>();

            string input_filepath1 = Tbk_Cmp_iFile1.Text;
            string input_filepath2 = Tbk_Cmp_iFile2.Text;
            string output_header_1 = Tbx_Cmp_oHeader1.Text;
            string output_header_2 = Tbx_Cmp_oHeader2.Text;
            string output_filename = Tbx_Cmp_oFilename.Text;
            string sepa_csv_char_o = Tbx_Cmp_CsvSeparator.Text;

            if (!File.Exists(input_filepath1) || !File.Exists(input_filepath2))
                errors.Add(mode + ERR_MISSING_IFILES);

            if (input_filepath1 != TBX_DEFAULT && input_filepath1 == input_filepath2)
                errors.Add(mode + ERR_SAME_IFILE);

            if (output_header_1 == "" || output_header_2 == "")
                errors.Add(mode + ERR_MISSING_OHDRS);

            if (output_filename == "")
                errors.Add(mode + ERR_MISSING_OFNAME);

            if (sepa_csv_char_o == "")
                errors.Add(mode + ERR_MISSING_CSVSEP);

            if (errors.Count > 0)
            {
                _ = MessageBox.Show(string.Join("\n\n", errors));
                return false;
            }
            return true;
        }

        private bool ValidatePrepareInputs()
        {
            string mode = "[Prepare mode] ";
            List<string> errors = new List<string>();

            string input_filepath1 = Tbk_Prp_iFile1.Text;
            string input_filepath2 = Tbk_Prp_iFile2.Text;
            string input_filepath3 = Tbk_Prp_iFile3.Text;
            string output_filename = Tbx_Prp_oFilename.Text;

            if (!File.Exists(input_filepath1) || !File.Exists(input_filepath2) || !File.Exists(input_filepath3))
                errors.Add(mode + ERR_MISSING_IFILES);

            if ((input_filepath1 != TBX_DEFAULT && input_filepath1 == input_filepath2) ||
                (input_filepath2 != TBX_DEFAULT && input_filepath2 == input_filepath3) ||
                (input_filepath3 != TBX_DEFAULT && input_filepath3 == input_filepath1))
                errors.Add(mode + ERR_SAME_IFILE);

            if (output_filename == "")
                errors.Add(mode + ERR_MISSING_OFNAME);

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
