using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace DS_TextsMod_Helper
{
    public partial class MainWindow : Window
    {

        #region CONSTANTS

        private const string DROP_FMG = "Drop FMG file...";

        private const string HDR_MISSING_IFILES = "Missing input files";
        private const string MSG_MISSING_IFILES = "Ensure that the input areas are holding files to process";

        private const string HDR_WRONG_IF_COUNT = "Wrong files count";
        private const string MSG_WRONG_IF_COUNT = "Input areas must share the same files count";

        private const string HDR_SAME_DIRECTORY = "Same directory";
        private const string MSG_SAME_DIRECTORY = "Files from distinct input areas cannot have the same directory";

        private const string HDR_INCONS_FMG_VER = "Inconsistent FMG versions";
        private const string MSG_INCONS_FMG_VER = "Ensure that the input files are compatible with each other";

        private const string HDR_OVERW_EXIST_OF = "Overwrite existing files";
        private const string MSG_OVERW_EXIST_OF = "The following output files already exist.\r\n"
                                                + "Continue to overwrite?";

        private const string WRN_DISTINCTS_FNAMES = "Warning : inconsistent filename(s).\r\n"
                                                  + "Make sure to use the right input files\r\n\r\n";

        private const string WRN_SPECIAL_CASES = "Warning : Found special cases whille processing files."
                                               + "\r\nSee details in file \"special cases.txt\"";

        private const string ERR_INVALID_FNAME = "Filenames cannot contain the following characters:\r\n"
                                               + "\\ / : * ? \" < > |\r\n\r\n"
                                               + "Please try again";

        private const string ERR_MISSING_IFILES = "Error : Missing input file(s)";
        private const string ERR_MISSING_OFNAME = "Error : Missing output filename";
        private const string ERR_MISSING_OHDRS = "Error : Missing output header(s)";
        private const string ERR_MISSING_CSVSEP = "Error : Missing CSV separator";
        private const string ERR_SAME_IFILE = "Error : Same file submitted several times";

        private const string ERR_MISSING_SFDLL = "Fatal error : file 'SoulsFormats.dll' not found";

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
            // Commented as building 1.5
            //if (Dtg_Preview.ItemsSource is List<ReadMode.Entry>)
            //    return PROCESS_MODE.Read;

            //if (Dtg_Preview.ItemsSource is List<CompareMode.Entry>)
            //    return PROCESS_MODE.Compare;

            //if (Dtg_Preview.ItemsSource is List<PrepareMode.Entry>)
            //    return PROCESS_MODE.Prepare;

            return PROCESS_MODE.None;
        }


        #endregion


        public MainWindow()
        {
            InitializeComponent();

            Title = Tools.GetFormattedAppVersion();

            Directory.CreateDirectory(Tools.GetOutputDirPath());

            if (!File.Exists(Tools.GetSoulsFormatsDllPath()))
                MessageBox.Show(ERR_MISSING_SFDLL);
        }


        #region GUI Common

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Keyboard.ClearFocus();
        }

        private void Brd_Drop(object sender, DragEventArgs e)
        {
            // Only files are accepted
            if (!e.Data.GetDataPresent(DataFormats.FileDrop))
                return;

            string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);
            ObservableCollection<InputFile> iFiles = RegisterInputFiles(droppedFiles);
            int validFilesCount = iFiles.Where(f => string.IsNullOrEmpty(f.Error)).ToList().Count();

            // Valid files are required
            if (validFilesCount == 0)
                return;

            Border brdSource = sender as Border;
            Label lbl = (Label)FindName("Lbl_" + brdSource.Tag);
            DataGrid dtg = (DataGrid)FindName("Dtg_" + brdSource.Tag);
            Border brd = (Border)FindName("Brd_" + brdSource.Tag);

            lbl.Visibility = Visibility.Collapsed;
            dtg.Visibility = Visibility.Visible;
            brd.Visibility = Visibility.Visible;

            HandleFilesDrop(dtg, iFiles);
        }

        private void Btn_ClearFiles_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            Border brd = (Border)FindName("Brd_" + btn.Tag);
            DataGrid dtg = (DataGrid)FindName("Dtg_" + btn.Tag);
            Label lbl = (Label)FindName("Lbl_" + btn.Tag);

            dtg.ItemsSource = null;

            brd.Visibility = Visibility.Collapsed;
            dtg.Visibility = Visibility.Collapsed;
            lbl.Visibility = Visibility.Visible;
        }

        private void Btn_MoveInputFileUp_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            DataGrid dtg = (DataGrid)FindName("Dtg_" + btn.Tag);

            int selectedCount = dtg.SelectedItems.Count;
            if (selectedCount == 0)
            {
                MessageBox.Show("No selection to reorder", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            List<int> positionsToMove = new List<int>();
            for (int i = 0; i < selectedCount; i++)
            {
                // $"Filename '{((InputFile)dtg.SelectedItems[i]).NameExt}' as selected item # {i + 1} is at index # {position} in the DataGrid\n";
                int position = dtg.Items.IndexOf(dtg.SelectedItems[i]);
                if (position == 0)
                    return;
                else
                    positionsToMove.Add(position);
            }

            ObservableCollection<InputFile> iFiles = (ObservableCollection<InputFile>)dtg.ItemsSource;
            for (int i = 0; i < iFiles.Count; i++)
            {
                if (positionsToMove.Contains(i)) // TODO(Multiple reordering): gather all necessary elements before using Insert/RemoveAt (or Build matching table as Dictionary or smthg?)
                {
                    InputFile toBeMovedDown = iFiles[i - 1];
                    iFiles.Insert(i - 1, iFiles[i]);
                    iFiles.RemoveAt(i);
                    iFiles.Insert(i, toBeMovedDown);
                    iFiles.RemoveAt(i + 1);
                }
            }

            dtg.ItemsSource = iFiles;
            dtg.SelectedIndex = positionsToMove.First() - 1; // Prevent losing the selected elements from altering elements order
        }

        private void Btn_MoveInputFileDown_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            DataGrid dtg = (DataGrid)FindName("Dtg_" + btn.Tag);

            int selectedCount = dtg.SelectedItems.Count;
            if (selectedCount == 0)
            {
                MessageBox.Show("No selection to reorder", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            List<int> positionsToMove = new List<int>();
            for (int i = 0; i < selectedCount; i++)
            {
                // $"Filename '{((InputFile)dtg.SelectedItems[i]).NameExt}' as selected item # {i + 1} is at index # {position} in the DataGrid\n";
                int position = dtg.Items.IndexOf(dtg.SelectedItems[i]);
                if (position == dtg.Items.Count - 1)
                    return;
                else
                    positionsToMove.Add(position);
            }

            ObservableCollection<InputFile> iFiles = (ObservableCollection<InputFile>)dtg.ItemsSource;
            for (int i = 0; i < iFiles.Count; i++)
            {
                if (positionsToMove.Contains(i)) // TODO(Multiple reordering): gather all necessary elements before using Insert/RemoveAt (or Build matching table as Dictionary or smthg?)
                {
                    InputFile toBeMovedDown = iFiles[i];
                    iFiles.Insert(i, iFiles[i + 1]);
                    iFiles.RemoveAt(i + 1);
                    iFiles.Insert(i + 1, toBeMovedDown);
                    iFiles.RemoveAt(i + 2);
                }
            }

            dtg.ItemsSource = iFiles;
            dtg.SelectedIndex = positionsToMove.First() + 1; // Prevent losing the selected elements from altering elements order
        }

        private void Dtg_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private void CompareInputFiles()
        {
            switch (SelectedMode())
            {


                case PROCESS_MODE.Compare:
                    if (Dtg_CmpA.ItemsSource is null || Dtg_CmpB.ItemsSource is null) return;

                    ObservableCollection<InputFile> iFilesCmpA = (ObservableCollection<InputFile>)Dtg_CmpA.ItemsSource;
                    ObservableCollection<InputFile> iFilesCmpB = (ObservableCollection<InputFile>)Dtg_CmpB.ItemsSource;
                    for (int i = 0; i < iFilesCmpA.Count; i++)
                    {
                        if (iFilesCmpA[i].NameExt == iFilesCmpB[i].NameExt)
                        {
                            // all good
                        }
                        else
                        {
                            // warning : filenames are different
                        }
                    }
                    break;


                case PROCESS_MODE.Prepare:
                    if (Dtg_PrpA.ItemsSource is null || Dtg_PrpB.ItemsSource is null || Dtg_PrpC.ItemsSource is null) return;

                    ObservableCollection<InputFile> iFilesPrpA = (ObservableCollection<InputFile>)Dtg_PrpA.ItemsSource;
                    ObservableCollection<InputFile> iFilesPrpB = (ObservableCollection<InputFile>)Dtg_PrpB.ItemsSource;
                    ObservableCollection<InputFile> iFilesPrpC = (ObservableCollection<InputFile>)Dtg_PrpC.ItemsSource;
                    for (int i = 0; i < iFilesPrpA.Count; i++)
                    {
                        if ((iFilesPrpA[i].NameExt == iFilesPrpB[i].NameExt) && (iFilesPrpB[i].NameExt == iFilesPrpC[i].NameExt))
                        {
                            // all good
                        }
                        else
                        {
                            // warning : filenames are different
                        }
                    }
                    break;


                default:
                    return;
            }
        }

        private void HandleFilesDrop(DataGrid targetDtg, ObservableCollection<InputFile> newFiles)
        {
            if (targetDtg.ItemsSource is ObservableCollection<InputFile>)
            {
                ObservableCollection<InputFile> currentFiles = (ObservableCollection<InputFile>)targetDtg.ItemsSource;
                foreach (InputFile newFile in newFiles)
                {
                    // Ignore new file when same filename or different directory;
                    if (currentFiles.Any(f => f.NameExt == newFile.NameExt) || currentFiles.Any(f => f.Directory != newFile.Directory))
                        continue;
                    currentFiles.Add(newFile);
                }
                targetDtg.ItemsSource = currentFiles;
            }
            else
                targetDtg.ItemsSource = newFiles;
        }



        #endregion


        #region GUI Read mode

        private void Btn_RdATest_Click(object sender, RoutedEventArgs e) // Preview files order
        {
            DataGrid dtg = Dtg_RdA;
            List<string> test = new List<string>();

            ObservableCollection<InputFile> iFiles = (ObservableCollection<InputFile>)dtg.ItemsSource;
            for (int i = 0; i < iFiles.Count; i++)
                test.Add($"{i + 1 } : {iFiles[i].NameExt}");

            MessageBox.Show($"From directory\n{iFiles.First().Directory}\n\n" + string.Join("\n", test));
        }

        #endregion


        #region GUI Compare mode

        // Something here ?

        #endregion


        #region GUI Prepare mode

        // Something here ?

        #endregion


        #region GUI Helpers : Manage tabs

        private void FocusMe(PROCESS_MODE targetMode)
        {
            if (SelectedMode() != targetMode)
                Tbc_Modes.SelectedIndex = (int)targetMode;
        }
        private void Tbi_Rd_DragOver(object sender, DragEventArgs e) { FocusMe(PROCESS_MODE.Read); }
        private void Tbi_Cmp_DragOver(object sender, DragEventArgs e) { FocusMe(PROCESS_MODE.Compare); }
        private void Tbi_Prp_DragOver(object sender, DragEventArgs e) { FocusMe(PROCESS_MODE.Prepare); }

        #endregion


        #region GUI Helpers : Process input files

        private void Tbk_InputFmg_PreviewDragOver(object sender, DragEventArgs e)
        {
            Activate();
            Focus();
            e.Handled = true;
        }

        private void Tbk_InputFmg_Drop(object sender, DragEventArgs e)
        {
            // Commented as building 1.5
            // Only files are accepted
            //if (!e.Data.GetDataPresent(DataFormats.FileDrop))
            //    return;

            //string[] droppedFiles = (string[])e.Data.GetData(DataFormats.FileDrop);

            //List<InputFile> iFiles = RegisterInputFiles(droppedFiles);
            //foreach (InputFile iFile in iFiles)
            //    ValidateInputFile((TextBlock)sender, iFile);

            //SyncFilenames(sender);
        }

        private ObservableCollection<InputFile> RegisterInputFiles(string[] droppedFiles)
        {
            ObservableCollection<InputFile> iFiles = new ObservableCollection<InputFile>();

            foreach (string filepath in droppedFiles)
                iFiles.Add(new InputFile(filepath));

            return iFiles;
        }

        private void ValidateInputFile(TextBlock tbk, InputFile iFile)
        {
            bool isValid = iFile.Error is null;

            if (isValid)
            {
                tbk.Inlines.Clear();
                tbk.Inlines.Add(new Run($"{iFile.Directory}\\"));
                tbk.Inlines.Add(new Bold(new Run($"{iFile.Name}.fmg")));

                tbk.FontStyle = FontStyles.Normal;
                tbk.Foreground = Brushes.Black;
            }
            else
            {
                tbk.Inlines.Clear();
                tbk.Inlines.Add(new Run(DROP_FMG));

                tbk.FontStyle = FontStyles.Italic;
                tbk.Foreground = Brushes.Gray;
            }

            tbk.ToolTip = iFile.GetToolTipText(isValid);

            // Store FMG versions of Prepare mode iFiles in hidden labels (lazy and hacky)
            if (SelectedMode() == PROCESS_MODE.Prepare)
            {
                Label lbl = Match_Tbk_Lbl(tbk);
                lbl.Content = iFile.VersionLg;
            }

            _ = ConfirmValidation(tbk, isValid);
        }

        /// <summary>
        /// Match Textblock (input file dropped) and hidden Label (FMG version)
        /// </summary>
        private Label Match_Tbk_Lbl(TextBlock tbk)
        {
            switch (tbk.Name)
            {
                // Commented as building 1.5
                //case "Tbk_Prp_iFile1": return Lbl_Prp_FmgVersion1;
                //case "Tbk_Prp_iFile2": return Lbl_Prp_FmgVersion2;
                //case "Tbk_Prp_iFile3": return Lbl_Prp_FmgVersion3;
                default: return new Label(); //never used default;
            }
        }

        private async Task ConfirmValidation(TextBlock tbk, bool fileIsValid) // TODO: Understand & Improve async from sync
        {
            SolidColorBrush softGreen = (SolidColorBrush)new BrushConverter().ConvertFrom("#c8eac8");

            tbk.Background = fileIsValid ? Brushes.LimeGreen : Brushes.Red;
            await Task.Delay(100);
            tbk.Background = fileIsValid ? softGreen : Brushes.White;

            CompareFilenames();
        }

        private void CompareFilenames()
        {
            // Commented as building 1.5
            //switch (SelectedMode())
            //{
            //    case PROCESS_MODE.Compare:

            //        List<TextBlock> cmpTbks = new List<TextBlock>() { Tbk_Cmp_iFile1, Tbk_Cmp_iFile2 };
            //        List<string> cmpNames = new List<string>();
            //        foreach (TextBlock tbk in cmpTbks)
            //        {
            //            if (tbk.Text == DROP_FMG) // || fakePaths.Contains(tbk.Text)) // DEBUG
            //                return;
            //            cmpNames.Add(Tools.GetFileName(tbk.Text));
            //        }

            //        // Count distinct values in 2 TextBlocks
            //        int cmpDistincts = cmpNames.Distinct().ToList().Count;
            //        switch (cmpDistincts)
            //        {
            //            case 1: foreach (TextBlock tbk in cmpTbks) ShowSameFilenames(tbk); break;
            //            case 2: foreach (TextBlock tbk in cmpTbks) ShowDistinctFilenames(tbk); break;
            //        }
            //        break;

            //    case PROCESS_MODE.Prepare:

            //        List<TextBlock> prpTbks = new List<TextBlock>() { Tbk_Prp_iFile1, Tbk_Prp_iFile2, Tbk_Prp_iFile3 };
            //        List<string> prpNames = new List<string>();
            //        foreach (TextBlock tbk in prpTbks)
            //        {
            //            if (tbk.Text == DROP_FMG) // || fakePaths.Contains(tbk.Text)) // DEBUG
            //                return;
            //            prpNames.Add(Tools.GetFileName(tbk.Text));
            //        }

            //        // Count distinct values in 3 TextBlocks
            //        int prpDistincts = prpNames.Distinct().ToList().Count;
            //        switch (prpDistincts)
            //        {
            //            case 1: foreach (TextBlock tbk in prpTbks) ShowSameFilenames(tbk); break;
            //            case 2: // Same behaviour for 2 and 3 distinct filenames
            //            case 3: foreach (TextBlock tbk in prpTbks) ShowDistinctFilenames(tbk); break;
            //        }
            //        break;
            //}
        }

        private void ShowSameFilenames(TextBlock tbk)
        {
            SolidColorBrush softGreen = (SolidColorBrush)new BrushConverter().ConvertFrom("#c8eac8");
            tbk.Background = softGreen;

            // Rely on ToolTip's text, quite hacky
            int warningLength = WRN_DISTINCTS_FNAMES.Length;
            if (tbk.ToolTip.ToString().Substring(0, warningLength) == WRN_DISTINCTS_FNAMES)
                tbk.ToolTip = tbk.ToolTip.ToString().Remove(0, warningLength);
        }

        private void ShowDistinctFilenames(TextBlock tbk)
        {
            tbk.Background = Brushes.PaleGoldenrod;

            // Rely on ToolTip's text, quite hacky
            int warningLength = WRN_DISTINCTS_FNAMES.Length;
            if (tbk.ToolTip.ToString().Substring(0, warningLength) != WRN_DISTINCTS_FNAMES)
                tbk.ToolTip = WRN_DISTINCTS_FNAMES + tbk.ToolTip.ToString();
        }

        /// <summary>
        /// Return FMG version to be used in output
        /// </summary>
        // Commented as building 1.5
        //private string GetOutputFmgVersion()
        //{

        //List<string> versions = new List<string>()
        //{
        //    Lbl_Prp_FmgVersion1.Content.ToString(),
        //    Lbl_Prp_FmgVersion2.Content.ToString(),
        //    Lbl_Prp_FmgVersion3.Content.ToString()
        //};
        //return versions.Distinct().ToList().Count > 1 ? SetOutputFileVersion(versions) : versions.First();
        //}


        public string SetOutputFileVersion(List<string> versions)
        {
            Label lblMessage = new Label()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                Background = Brushes.PaleGoldenrod,

                Content = new System.Text.StringBuilder()
                              .AppendLine("Submitted FMG input files belong to different Souls games.")
                              .Append("Please select the appropriate format to use in output :")
                              .ToString()
            };

            ListBox lbxChoices = new ListBox()
            {
                Margin = new Thickness(0, 8, 0, 8),
                ItemsSource = versions.Distinct().ToList()
            };

            Button btnConfirm = new Button()
            {
                Margin = new Thickness(8),
                Padding = new Thickness(6),
                Width = 64,
                Content = "Confirm"
            };

            StackPanel stkContainer = new StackPanel()
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Orientation = Orientation.Vertical
            };
            stkContainer.Children.Add(lblMessage);
            stkContainer.Children.Add(lbxChoices);
            stkContainer.Children.Add(btnConfirm);

            Window customDialog = new Window()
            {
                Content = stkContainer,
                Width = 360,
                Height = 220,
                Title = "Select output FMG version",
                ResizeMode = ResizeMode.NoResize,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            };

            string outputValue = "";
            btnConfirm.Click += (sender, e) =>
            {
                if (lbxChoices.SelectedItem is null)
                {
                    MessageBox.Show("Please select a FMG version");
                    return;
                }
                outputValue = lbxChoices.SelectedItem.ToString();
                customDialog.Close();
            };

            lbxChoices.MouseDoubleClick += (sender, e) =>
            {
                if (lbxChoices.SelectedItem is null)
                    return;

                outputValue = lbxChoices.SelectedItem.ToString();
                customDialog.Close();
            };

            customDialog.ShowDialog();
            return outputValue;
        }

        #endregion


        #region GUI Helpers : Misc.

        private void Btn_Explore_Click(object sender, RoutedEventArgs e)
        {
            // Commented as building 1.5
            //Button btn = sender as Button;
            //switch (btn.Name)
            //{
            //    case "Btn_Rd_ExploreFile1":
            //        Process.Start(Tbk_Rd_iFile1.Text != DROP_FMG ? Tools.GetParentDirPath(Tbk_Rd_iFile1.Text) : Tools.GetRootDirPath());
            //        break;
            //    case "Btn_Cmp_ExploreFile1":
            //        Process.Start(Tbk_Cmp_iFile1.Text != DROP_FMG ? Tools.GetParentDirPath(Tbk_Cmp_iFile1.Text) : Tools.GetRootDirPath());
            //        break;
            //    case "Btn_Cmp_ExploreFile2":
            //        Process.Start(Tbk_Cmp_iFile2.Text != DROP_FMG ? Tools.GetParentDirPath(Tbk_Cmp_iFile2.Text) : Tools.GetRootDirPath());
            //        break;
            //    case "Btn_Prp_ExploreFile1":
            //        Process.Start(Tbk_Prp_iFile1.Text != DROP_FMG ? Tools.GetParentDirPath(Tbk_Prp_iFile1.Text) : Tools.GetRootDirPath());
            //        break;
            //    case "Btn_Prp_ExploreFile2":
            //        Process.Start(Tbk_Prp_iFile2.Text != DROP_FMG ? Tools.GetParentDirPath(Tbk_Prp_iFile2.Text) : Tools.GetRootDirPath());
            //        break;
            //    case "Btn_Prp_ExploreFile3":
            //        Process.Start(Tbk_Prp_iFile3.Text != DROP_FMG ? Tools.GetParentDirPath(Tbk_Prp_iFile3.Text) : Tools.GetRootDirPath());
            //        break;
            //    default:
            //        Process.Start(Tools.GetRootDirPath());
            //        break;
            //}
        }

        private bool FilenameIsValid(string text)
        {
            List<char> invalidChars = new List<char>() { '\\', '/', ':', '*', '?', '"', '<', '>', '|' };

            foreach (char c in invalidChars)
                if (text.Contains(c))
                    return false;

            return true;
        }

        #endregion


        #region Preview

        // Commented as building 1.5
        //private void Btn_RefreshPreview_Click(object sender, RoutedEventArgs e)
        //{
        //    switch (SelectedMode())
        //    {
        //        case PROCESS_MODE.Read: PreviewRead(); break;
        //        case PROCESS_MODE.Compare: PreviewCompare(); break;
        //        case PROCESS_MODE.Prepare: PreviewPrepare(); break;
        //    }
        //}

        // Commented as building 1.5
        //private void Cbx_PreviewAllDetails_Checked(object sender, RoutedEventArgs e)
        //{
        //    switch (LoadedMode())
        //    {
        //        case PROCESS_MODE.None: return;
        //        case PROCESS_MODE.Read: PreviewRead((List<ReadMode.Entry>)Dtg_Preview.ItemsSource); break;
        //        case PROCESS_MODE.Compare: PreviewCompare((List<CompareMode.Entry>)Dtg_Preview.ItemsSource); break;
        //        case PROCESS_MODE.Prepare: PreviewPrepare((List<PrepareMode.Entry>)Dtg_Preview.ItemsSource); break;
        //    }
        //}

        // Commented as building 1.5
        //private void Cbx_PreviewAllDetails_Unchecked(object sender, RoutedEventArgs e)
        //{
        //    switch (LoadedMode())
        //    {
        //        case PROCESS_MODE.None: return;
        //        case PROCESS_MODE.Read: PreviewRead((List<ReadMode.Entry>)Dtg_Preview.ItemsSource); break;
        //        case PROCESS_MODE.Compare: PreviewCompare((List<CompareMode.Entry>)Dtg_Preview.ItemsSource); break;
        //        case PROCESS_MODE.Prepare: PreviewPrepare((List<PrepareMode.Entry>)Dtg_Preview.ItemsSource); break;
        //    }
        //}

        // Commented as building 1.5
        //private void PreviewRead(List<ReadMode.Entry> r_entries = null)
        //{
        //    if (r_entries is null)
        //    {
        //        string iFile1 = Tbk_Rd_iFile1.Text;

        //        if (iFile1 == DROP_FMG)
        //        {
        //            MessageBox.Show("[Read mode] " + ERR_MISSING_IFILES);
        //            return;
        //        }

        //        ReadMode r = new ReadMode(iFile1) { OneLinedValues = Cbx_Rd_OneLinedValues.IsChecked ?? false };
        //        r.ProcessFiles(true);
        //        r_entries = r.Entries;
        //    }

        //    Dtg_Preview.Visibility = Visibility.Visible;
        //    Dtg_Preview.Columns.Clear();

        //    bool allDetails = Cbx_PreviewAllDetails.IsChecked ?? false;
        //    bool detached = false;

        //    List<DataGridTextColumn> columns = GetReadColumns(allDetails, detached);
        //    foreach (DataGridTextColumn col in columns)
        //        Dtg_Preview.Columns.Add(col);

        //    Dtg_Preview.ItemsSource = r_entries;

        //}

        // Commented as building 1.5
        //private void PreviewCompare(List<CompareMode.Entry> c_entries = null)
        //{
        //    if (c_entries is null)
        //    {
        //        string iFile1 = Tbk_Cmp_iFile1.Text;
        //        string iFile2 = Tbk_Cmp_iFile2.Text;

        //        if (iFile1 == DROP_FMG || iFile2 == DROP_FMG)
        //        {
        //            MessageBox.Show("[Compare mode] " + ERR_MISSING_IFILES);
        //            return;
        //        }

        //        CompareMode c = new CompareMode(iFile1, iFile2) { OneLinedValues = Cbx_Cmp_OneLinedValues.IsChecked ?? false };
        //        c.ProcessFiles(true);
        //        c_entries = c.Entries;
        //    }

        //    Dtg_Preview.Visibility = Visibility.Visible;
        //    Dtg_Preview.Columns.Clear();

        //    bool allDetails = Cbx_PreviewAllDetails.IsChecked ?? false;
        //    bool detached = false;

        //    List<DataGridTextColumn> columns = GetCompareColumns(allDetails, detached);
        //    foreach (DataGridTextColumn col in columns)
        //        Dtg_Preview.Columns.Add(col);

        //    Dtg_Preview.ItemsSource = c_entries;
        //}

        // Commented as building 1.5
        //private void PreviewPrepare(List<PrepareMode.Entry> p_entries = null)
        //{
        //    if (p_entries is null)
        //    {
        //        string iFile1 = Tbk_Prp_iFile1.Text;
        //        string iFile2 = Tbk_Prp_iFile2.Text;
        //        string iFile3 = Tbk_Prp_iFile3.Text;
        //        string textToReplace = Tbx_Prp_TextToReplace.Text;
        //        string replacingText = Tbx_Prp_ReplacingText.Text;

        //        if (iFile1 == DROP_FMG || iFile2 == DROP_FMG || iFile3 == DROP_FMG)
        //        {
        //            MessageBox.Show("[Prepare mode] " + ERR_MISSING_IFILES);
        //            Cbx_PreviewAllDetails.IsChecked = false;
        //            return;
        //        }

        //        PrepareMode p = new PrepareMode(iFile1, iFile2, iFile3, textToReplace, replacingText);
        //        p.ProcessFiles(true);
        //        p_entries = p.Entries;
        //    }

        //    Dtg_Preview.Visibility = Visibility.Visible;
        //    Dtg_Preview.Columns.Clear();

        //    bool allDetails = Cbx_PreviewAllDetails.IsChecked ?? false;
        //    bool detached = false;

        //    List<DataGridTextColumn> columns = GetPrepareColumns(allDetails, detached);
        //    foreach (DataGridTextColumn col in columns)
        //        Dtg_Preview.Columns.Add(col);

        //    Dtg_Preview.ItemsSource = p_entries;
        //}

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
            int processedFilesCount = 0;
            List<string> iFilenames;
            List<string> alreadyExisting;
            string parentDirPathA;
            string parentDirPathB;
            string parentDirPathC;
            string fileFmgVersionA;
            string fileFmgVersionB;
            string fileFmgVersionC;
            string filePathA;
            string filePathB;
            string filePathC;
            string csvSepChar;

            switch (SelectedMode())
            {
                case PROCESS_MODE.Read:
                    if (Dtg_RdA.ItemsSource is null)
                    {   // Early return on error "Missing input files"
                        MessageBox.Show(MSG_MISSING_IFILES, HDR_MISSING_IFILES, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    ObservableCollection<InputFile> iFilesRd = (ObservableCollection<InputFile>)Dtg_RdA.ItemsSource;
                    parentDirPathA = iFilesRd.First().Directory;

                    iFilenames = iFilesRd.Select(iFile => iFile.Name + ".csv").ToList();
                    alreadyExisting = Tools.GetAlreadyExistingFilenames(iFilenames);
                    if (alreadyExisting.Count > 0)
                    {
                        string fnames = string.Join("\r\n - ", alreadyExisting);
                        MessageBoxResult mbr = MessageBox.Show(MSG_OVERW_EXIST_OF + $"\r\n\r\n{fnames}", HDR_OVERW_EXIST_OF, MessageBoxButton.OKCancel, MessageBoxImage.Information);
                        if (mbr == MessageBoxResult.Cancel)
                            return;
                    }

                    foreach (InputFile iFile in iFilesRd)
                    {
                        filePathA = Path.Combine(parentDirPathA, iFile.NameExt);
                        csvSepChar = Tbx_Rd_CsvSeparator.Text;
                        ReadMode r = new ReadMode(filePathA) { OneLinedValues = Cbx_Rd_OneLinedValues.IsChecked ?? false };
                        r.ProcessFiles(false);
                        r.ProduceOutput(iFile.Name + ".csv", csvSepChar);
                        processedFilesCount += 1;
                    }
                    MessageBox.Show($"[Read mode] Done: {processedFilesCount} output files have been created");
                    break;


                case PROCESS_MODE.Compare:
                    if (Dtg_CmpA.ItemsSource is null || Dtg_CmpB.ItemsSource is null)
                    {   // Early return on error "Missing input files"
                        MessageBox.Show(MSG_MISSING_IFILES, HDR_MISSING_IFILES, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    ObservableCollection<InputFile> iFilesCmpA = (ObservableCollection<InputFile>)Dtg_CmpA.ItemsSource;
                    ObservableCollection<InputFile> iFilesCmpB = (ObservableCollection<InputFile>)Dtg_CmpB.ItemsSource;
                    if (iFilesCmpA.Count != iFilesCmpB.Count)
                    {   // Early return on error "Wrong input files count"
                        MessageBox.Show(MSG_WRONG_IF_COUNT, HDR_WRONG_IF_COUNT, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    parentDirPathA = iFilesCmpA.First().Directory;
                    parentDirPathB = iFilesCmpB.First().Directory;
                    if (parentDirPathA == parentDirPathB)
                    {   // Early return on error "Same directory"
                        MessageBox.Show(MSG_SAME_DIRECTORY, HDR_SAME_DIRECTORY, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    fileFmgVersionA = iFilesCmpA.First().VersionLg;
                    fileFmgVersionB = iFilesCmpB.First().VersionLg;
                    if (fileFmgVersionA != fileFmgVersionB)
                    {   // Early return on error "Inconsistent FMG versions"
                        MessageBox.Show(MSG_INCONS_FMG_VER, HDR_INCONS_FMG_VER, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    iFilenames = iFilesCmpA.Select(iFile => iFile.Name + ".csv").ToList();
                    alreadyExisting = Tools.GetAlreadyExistingFilenames(iFilenames);
                    if (alreadyExisting.Count > 0)
                    {
                        string fnames = string.Join("\r\n - ", alreadyExisting);
                        MessageBoxResult mbr = MessageBox.Show(MSG_OVERW_EXIST_OF + $"\r\n\r\n{fnames}", HDR_OVERW_EXIST_OF, MessageBoxButton.OKCancel, MessageBoxImage.Information);
                        if (mbr == MessageBoxResult.Cancel)
                            return;
                    }

                    for (int i = 0; i < iFilesCmpA.Count; i++)
                    {
                        filePathA = Path.Combine(parentDirPathA, iFilesCmpA[i].NameExt);
                        filePathB = Path.Combine(parentDirPathB, iFilesCmpB[i].NameExt);
                        csvSepChar = Tbx_Cmp_CsvSeparator.Text;
                        CompareMode c = new CompareMode(filePathA, filePathB) { OneLinedValues = Cbx_Cmp_OneLinedValues.IsChecked ?? false };
                        c.ProcessFiles(false);
                        c.ProduceOutput(iFilesCmpA[i].Name + ".csv", Tbx_Cmp_oHeader1.Text, Tbx_Cmp_oHeader2.Text, csvSepChar);
                        processedFilesCount += 1;
                    }
                    MessageBox.Show($"[Compare mode] Done: {processedFilesCount} output files have been created");
                    break;


                case PROCESS_MODE.Prepare:
                    if (Dtg_PrpA.ItemsSource is null || Dtg_PrpB.ItemsSource is null || Dtg_PrpC.ItemsSource is null)
                    {   // Early return on error "Missing input files"
                        MessageBox.Show(MSG_MISSING_IFILES, HDR_MISSING_IFILES, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    ObservableCollection<InputFile> iFilesPrpA = (ObservableCollection<InputFile>)Dtg_PrpA.ItemsSource;
                    ObservableCollection<InputFile> iFilesPrpB = (ObservableCollection<InputFile>)Dtg_PrpB.ItemsSource;
                    ObservableCollection<InputFile> iFilesPrpC = (ObservableCollection<InputFile>)Dtg_PrpC.ItemsSource;
                    if (iFilesPrpA.Count != iFilesPrpB.Count || iFilesPrpA.Count != iFilesPrpC.Count)
                    {   // Early return on error "Wrong input files count"
                        MessageBox.Show(MSG_WRONG_IF_COUNT, HDR_WRONG_IF_COUNT, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    parentDirPathA = iFilesPrpA.First().Directory;
                    parentDirPathB = iFilesPrpB.First().Directory;
                    parentDirPathC = iFilesPrpC.First().Directory;
                    if (parentDirPathA == parentDirPathB || parentDirPathA == parentDirPathC)
                    {   // Early return on error "Same directory"
                        MessageBox.Show(MSG_SAME_DIRECTORY, HDR_SAME_DIRECTORY, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }
                    fileFmgVersionA = iFilesPrpA.First().VersionLg;
                    fileFmgVersionB = iFilesPrpB.First().VersionLg;
                    fileFmgVersionC = iFilesPrpC.First().VersionLg;
                    if (fileFmgVersionA != fileFmgVersionB || fileFmgVersionA != fileFmgVersionC)
                    {   // Early return on error "Inconsistent FMG versions"
                        MessageBox.Show(MSG_INCONS_FMG_VER, HDR_INCONS_FMG_VER, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                    }

                    iFilenames = iFilesPrpA.Select(iFile => iFile.NameExt).ToList();
                    alreadyExisting = Tools.GetAlreadyExistingFilenames(iFilenames);
                    if (alreadyExisting.Count > 0)
                    {
                        string fnames = string.Join("\r\n", alreadyExisting);
                        MessageBoxResult mbr = MessageBox.Show(MSG_OVERW_EXIST_OF + $"\r\n\r\n{fnames}", HDR_OVERW_EXIST_OF, MessageBoxButton.OKCancel, MessageBoxImage.Information);
                        if (mbr == MessageBoxResult.Cancel)
                            return;
                    }

                    bool haveSpecialCases = false;
                    for (int i = 0; i < iFilesPrpA.Count; i++)
                    {
                        filePathA = Path.Combine(parentDirPathA, iFilesPrpA[i].NameExt);
                        filePathB = Path.Combine(parentDirPathB, iFilesPrpB[i].NameExt);
                        filePathC = Path.Combine(parentDirPathC, iFilesPrpC[i].NameExt);
                        PrepareMode p = new PrepareMode(filePathA, filePathB, filePathC, Tbx_Prp_TextToReplace.Text, Tbx_Prp_ReplacingText.Text);
                        p.ProcessFiles(false);
                        p.SetOutputVersion(fileFmgVersionA);
                        p.ProduceOutput(iFilesPrpA[i].NameExt);
                        if (Cbx_Prp_WarnOnSpecialCases.IsChecked ?? false)
                        {
                            List<string> specialCases = p.GetSpecialCases();
                            if (specialCases.Count > 0)
                            {
                                Tools.LogSpecialCases(filePathA, filePathB, filePathC, iFilesPrpA[i].NameExt, specialCases);
                                haveSpecialCases = true;
                            }
                        }
                        processedFilesCount += 1;
                    }
                    string special = haveSpecialCases ? $"\n\n{WRN_SPECIAL_CASES}" : string.Empty;
                    MessageBox.Show($"[Prepare mode] Done: {processedFilesCount} output files have been created{special}");
                    break;
            }


            // Commented as building 1.5
            //switch (SelectedMode())
            //{
            //    case PROCESS_MODE.Read:

            //        if (!ValidateReadInputs())
            //            return;

            //        string rd_iFile1 = Tbk_Rd_iFile1.Text;
            //        string rd_oFilename = Tbx_Rd_oFilename.Text + ".csv";
            //        string rd_csvSepChar = Tbx_Rd_CsvSeparator.Text;

            //        if (File.Exists(Tools.GetOutputFilepath(rd_oFilename)))
            //        {
            //            MessageBoxResult result = MessageBox.Show(MSG_EXISTING_OFNAME, HDR_EXISTING_OFNAME, MessageBoxButton.OKCancel, MessageBoxImage.Information);
            //            if (result == MessageBoxResult.Cancel)
            //                return;
            //        }

            //        ReadMode r = new ReadMode(rd_iFile1) { OneLinedValues = Cbx_Rd_OneLinedValues.IsChecked ?? false };
            //        r.ProcessFiles(false);
            //        r.ProduceOutput(rd_oFilename, rd_csvSepChar);

            //        MessageBox.Show($"[Read mode] File \"{r.OutputFilename}\" created");
            //        break;

            //    case PROCESS_MODE.Compare:

            //        if (!ValidateCompareInputs())
            //            return;

            //        string cmp_iFile1 = Tbk_Cmp_iFile1.Text;
            //        string cmp_iFile2 = Tbk_Cmp_iFile2.Text;
            //        string cmp_oFilename = Tbx_Cmp_oFilename.Text + ".csv";
            //        string oHdr1 = Tbx_Cmp_oHeader1.Text;
            //        string oHdr2 = Tbx_Cmp_oHeader2.Text;
            //        string cmp_csvSepChar = Tbx_Cmp_CsvSeparator.Text;

            //        if (File.Exists(Tools.GetOutputFilepath(cmp_oFilename)))
            //        {
            //            MessageBoxResult result = MessageBox.Show(MSG_EXISTING_OFNAME, HDR_EXISTING_OFNAME, MessageBoxButton.OKCancel, MessageBoxImage.Information);
            //            if (result == MessageBoxResult.Cancel)
            //                return;
            //        }

            //        CompareMode c = new CompareMode(cmp_iFile1, cmp_iFile2) { OneLinedValues = Cbx_Cmp_OneLinedValues.IsChecked ?? false };
            //        c.ProcessFiles(false);
            //        c.ProduceOutput(cmp_oFilename, oHdr1, oHdr2, cmp_csvSepChar);

            //        MessageBox.Show($"[Compare mode] File \"{c.OutputFilename}\" created");
            //        break;

            //    case PROCESS_MODE.Prepare:

            //        if (!ValidatePrepareInputs())
            //            return;

            //        string prp_iFile1 = Tbk_Prp_iFile1.Text;
            //        string prp_iFile2 = Tbk_Prp_iFile2.Text;
            //        string prp_iFile3 = Tbk_Prp_iFile3.Text;
            //        string prp_oFilename = Tbx_Prp_oFilename.Text + ".fmg";
            //        string textToReplace = Tbx_Prp_TextToReplace.Text;
            //        string replacingText = Tbx_Prp_ReplacingText.Text;

            //        string outputVersion = GetOutputFmgVersion();
            //        if (string.IsNullOrEmpty(outputVersion))
            //            return;

            //        if (File.Exists(Tools.GetOutputFilepath(prp_oFilename)))
            //        {
            //            MessageBoxResult result = MessageBox.Show(MSG_EXISTING_OFNAME, HDR_EXISTING_OFNAME, MessageBoxButton.OKCancel, MessageBoxImage.Information);
            //            if (result == MessageBoxResult.Cancel)
            //                return;
            //        }

            //        PrepareMode p = new PrepareMode(prp_iFile1, prp_iFile2, prp_iFile3, textToReplace, replacingText);
            //        p.ProcessFiles(false);
            //        p.SetOutputVersion(outputVersion);
            //        p.ProduceOutput(prp_oFilename);

            //        if (Cbx_Prp_WarnOnSpecialCases.IsChecked ?? false)
            //        {
            //            List<string> specialCases = p.GetSpecialCases();
            //            if (specialCases.Count > 0)
            //            {
            //                Tools.LogSpecialCases(prp_iFile1, prp_iFile2, prp_iFile3, prp_oFilename, specialCases);
            //                MessageBox.Show(WRN_SPECIAL_CASES);
            //            }
            //        }

            //        MessageBox.Show($"[Prepare mode] File \"{p.OutputFilename}\" created for \"{outputVersion}\"");
            //        break;
            //}
        }


        // Commented as building 1.5
        //private bool ValidateReadInputs()
        //{
        //    string mode = "[Read mode] ";
        //    List<string> errors = new List<string>();

        //    string input_filepath1 = Tbk_Rd_iFile1.Text;
        //    string output_filename = Tbx_Rd_oFilename.Text;
        //    string sepa_csv_char_o = Tbx_Rd_CsvSeparator.Text;

        //    if (!File.Exists(input_filepath1))
        //        errors.Add(mode + ERR_MISSING_IFILES);

        //    if (output_filename == "")
        //        errors.Add(mode + ERR_MISSING_OFNAME);

        //    if (sepa_csv_char_o == "")
        //        errors.Add(mode + ERR_MISSING_CSVSEP);

        //    if (errors.Count > 0)
        //    {
        //        MessageBox.Show(string.Join("\n\n", errors));
        //        return false;
        //    }
        //    return true;
        //}

        // Commented as building 1.5
        //private bool ValidateCompareInputs()
        //{
        //    string mode = "[Compare mode] ";
        //    List<string> errors = new List<string>();

        //    string input_filepath1 = Tbk_Cmp_iFile1.Text;
        //    string input_filepath2 = Tbk_Cmp_iFile2.Text;
        //    string output_header_1 = Tbx_Cmp_oHeader1.Text;
        //    string output_header_2 = Tbx_Cmp_oHeader2.Text;
        //    string output_filename = Tbx_Cmp_oFilename.Text;
        //    string sepa_csv_char_o = Tbx_Cmp_CsvSeparator.Text;

        //    if (!File.Exists(input_filepath1) || !File.Exists(input_filepath2))
        //        errors.Add(mode + ERR_MISSING_IFILES);

        //    if (input_filepath1 != DROP_FMG && input_filepath1 == input_filepath2)
        //        errors.Add(mode + ERR_SAME_IFILE);

        //    if (output_header_1 == "" || output_header_2 == "")
        //        errors.Add(mode + ERR_MISSING_OHDRS);

        //    if (output_filename == "")
        //        errors.Add(mode + ERR_MISSING_OFNAME);

        //    if (sepa_csv_char_o == "")
        //        errors.Add(mode + ERR_MISSING_CSVSEP);

        //    if (errors.Count > 0)
        //    {
        //        MessageBox.Show(string.Join("\n\n", errors));
        //        return false;
        //    }
        //    return true;
        //}

        // Commented as building 1.5
        //private bool ValidatePrepareInputs()
        //{
        //    string mode = "[Prepare mode] ";
        //    List<string> errors = new List<string>();

        //    string input_filepath1 = Tbk_Prp_iFile1.Text;
        //    string input_filepath2 = Tbk_Prp_iFile2.Text;
        //    string input_filepath3 = Tbk_Prp_iFile3.Text;
        //    string output_filename = Tbx_Prp_oFilename.Text;

        //    if (!File.Exists(input_filepath1) || !File.Exists(input_filepath2) || !File.Exists(input_filepath3))
        //        errors.Add(mode + ERR_MISSING_IFILES);

        //    if ((input_filepath1 != DROP_FMG && input_filepath1 == input_filepath2) ||
        //        (input_filepath2 != DROP_FMG && input_filepath2 == input_filepath3) ||
        //        (input_filepath3 != DROP_FMG && input_filepath3 == input_filepath1))
        //        errors.Add(mode + ERR_SAME_IFILE);

        //    if (output_filename == "")
        //        errors.Add(mode + ERR_MISSING_OFNAME);

        //    if (errors.Count > 0)
        //    {
        //        MessageBox.Show(string.Join("\n\n", errors));
        //        return false;
        //    }
        //    return true;
        //}

        #endregion

    }
}
