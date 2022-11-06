using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DS_TextsMod_Helper
{
    public partial class MainWindow : Window
    {

        #region CONSTANTS

        private const string HDR_MISSING_IFILES = "Missing input files";
        private const string MSG_MISSING_IFILES = "All input areas must contain files to process";

        private const string HDR_WRONG_IFCOUNT = "Wrong files count";
        private const string MSG_WRONG_IFCOUNT = "Input areas must share the same files count";

        private const string HDR_SAME_DIRECTORY = "Same parent directory";
        private const string MSG_SAME_DIRECTORY = "Files from distinct input areas cannot have the same parent directory";

        private const string HDR_INCONS_FMG_VER = "Inconsistent FMG versions";
        private const string MSG_INCONS_FMG_VER = "Input files must be compatible with each other (\"Type\" must match)";

        private const string HDR_OVERW_EXIST_OF = "Overwrite existing files";
        private const string MSG_OVERW_EXIST_OF = "The following output files already exist.\r\n"
                                                + "Continue and overwrite them?";

        private const string HDR_INCONS_IFNAMES = "Inconsistent input filenames";
        private const string MSG_INCONS_IFNAMES1 = "Filenames are different on the following lines :";
        private const string MSG_INCONS_IFNAMES2 = "Continue anyway?";

        private const string WRN_SPECIAL_CASES = "Warning : Found special cases while processing files.\r\n"
                                               + "See details in file \"special cases.txt\"";

        private const string ERR_MISSING_SFDLL = "Fatal error : file 'SoulsFormats.dll' not found.\r\n"
                                               + "The program will exit...";

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

        #endregion


        public MainWindow()
        {
            InitializeComponent();

            if (!File.Exists(Tools.GetSoulsFormatsDllPath()))
            {
                MessageBox.Show(ERR_MISSING_SFDLL);
                Environment.Exit(0);
            }

            Title = Tools.GetFormattedAppVersion();
            Directory.CreateDirectory(Tools.GetOutputDirPath());
        }


        #region Input files management

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

        private void Btn_OpenParentDir_Click(object sender, RoutedEventArgs e)
        {
            Button btnSource = sender as Button;
            DataGrid dtg = (DataGrid)FindName("Dtg_" + btnSource.Tag);
            ObservableCollection<InputFile> iFiles = (ObservableCollection<InputFile>)dtg.ItemsSource;

            Process.Start(iFiles.First().Directory);
        }

        private void Btn_MoveInputFileUp_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            DataGrid dtg = (DataGrid)FindName("Dtg_" + btn.Tag);

            int selectedCount = dtg.SelectedItems.Count;
            if (selectedCount == 0)
            {
                MessageBox.Show("No file selected", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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

            dtg.ItemsSource = null;
            dtg.ItemsSource = iFiles;
            dtg.SelectedIndex = positionsToMove.First() - 1; // Prevent losing the selected elements from altering elements order
            dtg.Focus();
        }

        private void Btn_RemoveFiles_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            DataGrid dtg = (DataGrid)FindName("Dtg_" + btn.Tag);

            int selectedCount = dtg.SelectedItems.Count;
            if (selectedCount == 0)
            {
                MessageBox.Show("No file selected", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            List<int> positionsToRemove = new List<int>();
            for (int i = 0; i < selectedCount; i++)
            {
                // $"Filename '{((InputFile)dtg.SelectedItems[i]).NameExt}' as selected item # {i + 1} is at index # {position} in the DataGrid\n";
                int position = dtg.Items.IndexOf(dtg.SelectedItems[i]);
                positionsToRemove.Add(position);
            }

            ObservableCollection<InputFile> iFiles = (ObservableCollection<InputFile>)dtg.ItemsSource;
            int iFilesCount = iFiles.Count;
            for (int i = 0; i < iFilesCount; i++)
                if (positionsToRemove.Contains(i))
                    iFiles.RemoveAt(i);

            dtg.ItemsSource = null;
            dtg.ItemsSource = iFiles;
            dtg.SelectedIndex = positionsToRemove.Contains(iFilesCount) ? -1 : positionsToRemove.Max();
            dtg.Focus();
        }

        private void Btn_MoveInputFileDown_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            DataGrid dtg = (DataGrid)FindName("Dtg_" + btn.Tag);

            int selectedCount = dtg.SelectedItems.Count;
            if (selectedCount == 0)
            {
                MessageBox.Show("No file selected", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
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
            dtg.Focus();
        }

        private void Dtg_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }

        private List<int> CompareFilenames(ObservableCollection<InputFile> ifA, ObservableCollection<InputFile> ifB, ObservableCollection<InputFile> ifC = null)
        {
            List<int> lineNumbers = new List<int>();
            switch (SelectedMode())
            {
                case PROCESS_MODE.Compare:
                    for (int i = 0; i < ifA.Count; i++)
                    {
                        if (ifA[i].NameExt != ifB[i].NameExt)
                        {
                            lineNumbers.Add(i + 1); // Filenames are different : add faulty line
                        }
                    }
                    break;

                case PROCESS_MODE.Prepare:
                    for (int i = 0; i < ifA.Count; i++)
                    {
                        if (ifA[i].NameExt != ifB[i].NameExt || ifA[i].NameExt != ifC[i].NameExt)
                        {
                            lineNumbers.Add(i + 1); // Filenames are different : add faulty line
                        }
                    }
                    break;
            }
            return lineNumbers;
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

        private ObservableCollection<InputFile> RegisterInputFiles(string[] droppedItems)
        {
            ObservableCollection<InputFile> iFiles = new ObservableCollection<InputFile>();
            foreach (string path in droppedItems)
            {
                if (Directory.Exists(path))
                {
                    DirectoryInfo di = new DirectoryInfo(path);
                    foreach (FileInfo fi in di.GetFiles())
                    {
                        iFiles.Add(new InputFile(fi.FullName));
                    }
                }
                else
                {
                    iFiles.Add(new InputFile(path));
                }
            }
            return iFiles;
        }

        #endregion


        #region TabItem controls management

        private void FocusMe(PROCESS_MODE targetMode)
        {
            if (SelectedMode() != targetMode)
                Tbc_Modes.SelectedIndex = (int)targetMode;
        }
        private void Tbi_Rd_DragOver(object sender, DragEventArgs e) { FocusMe(PROCESS_MODE.Read); }
        private void Tbi_Cmp_DragOver(object sender, DragEventArgs e) { FocusMe(PROCESS_MODE.Compare); }
        private void Tbi_Prp_DragOver(object sender, DragEventArgs e) { FocusMe(PROCESS_MODE.Prepare); }

        #endregion


        #region GUI helpers : Misc.

        private void Btn_OpenProgramDir_Click(object sender, RoutedEventArgs e)
        {
            Process.Start(Tools.GetRootDirPath());
        }

        #endregion


        #region Preview

        private void Btn_ShowOutputPreview_Click(object sender, RoutedEventArgs e) // TODO: Factorize with Output common elements
        {
            int processedFilesCount = 0;
            string parentDirPathA;
            string parentDirPathB;
            string parentDirPathC;
            string fileFmgVersionA;
            string fileFmgVersionB;
            string fileFmgVersionC;
            string filePathA;
            string filePathB;
            string filePathC;
            List<int> linesWithDistinctFilenames;
            ProcessingMode processingMode;
            OutputPreview outputPreview;

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

                    List<ReadMode> rdSuperlist = new List<ReadMode>();
                    foreach (InputFile iFile in iFilesRd)
                    {
                        filePathA = Path.Combine(parentDirPathA, iFile.NameExt);
                        ReadMode r = new ReadMode(filePathA)
                        {
                            Title = $"{processedFilesCount + 1}: {iFile.Name}",
                            OneLinedValues = Cbx_Rd_OneLinedValues.IsChecked ?? false
                        };
                        r.ProcessFiles(true);
                        rdSuperlist.Add(r);
                        processedFilesCount += 1;
                    }
                    processingMode = new ProcessingMode() { AllReadModeEntries = rdSuperlist };
                    outputPreview = new OutputPreview(processingMode);
                    outputPreview.ShowDialog();
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
                        MessageBox.Show(MSG_WRONG_IFCOUNT, HDR_WRONG_IFCOUNT, MessageBoxButton.OK, MessageBoxImage.Error);
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
                    linesWithDistinctFilenames = CompareFilenames(iFilesCmpA, iFilesCmpB);
                    if (linesWithDistinctFilenames.Count != 0)
                    {   // Warning if filenames are not the same in all input areas
                        string lines = string.Join(", ", linesWithDistinctFilenames);
                        MessageBoxResult mbr = MessageBox.Show(
                            MSG_INCONS_IFNAMES1 + $"\r\n{lines}\r\n\r\n" + MSG_INCONS_IFNAMES2, HDR_INCONS_IFNAMES, MessageBoxButton.OKCancel, MessageBoxImage.Information
                        );
                        if (mbr == MessageBoxResult.Cancel)
                            return;
                    }

                    List<CompareMode> cmpSuperlist = new List<CompareMode>();
                    for (int i = 0; i < iFilesCmpA.Count; i++)
                    {
                        filePathA = Path.Combine(parentDirPathA, iFilesCmpA[i].NameExt);
                        filePathB = Path.Combine(parentDirPathB, iFilesCmpB[i].NameExt);
                        CompareMode c = new CompareMode(filePathA, filePathB)
                        {
                            Title = $"{processedFilesCount + 1}: {iFilesCmpA[i].Name}",
                            OneLinedValues = Cbx_Cmp_OneLinedValues.IsChecked ?? false,
                            OutputHeaderA = Tbx_Cmp_oHeader1.Text,
                            OutputHeaderB = Tbx_Cmp_oHeader2.Text
                        };
                        c.ProcessFiles(true);
                        cmpSuperlist.Add(c);
                        processedFilesCount += 1;
                    }
                    processingMode = new ProcessingMode() { AllCompareModeEntries = cmpSuperlist };
                    outputPreview = new OutputPreview(processingMode);
                    outputPreview.ShowDialog();
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
                        MessageBox.Show(MSG_WRONG_IFCOUNT, HDR_WRONG_IFCOUNT, MessageBoxButton.OK, MessageBoxImage.Error);
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
                    linesWithDistinctFilenames = CompareFilenames(iFilesPrpA, iFilesPrpB, iFilesPrpC);
                    if (linesWithDistinctFilenames.Count != 0)
                    {   // Warning if filenames are not the same in all input areas
                        string lines = string.Join(", ", linesWithDistinctFilenames);
                        MessageBoxResult mbr = MessageBox.Show(
                            MSG_INCONS_IFNAMES1 + $"\r\n{lines}\r\n\r\n" + MSG_INCONS_IFNAMES2, HDR_INCONS_IFNAMES, MessageBoxButton.OKCancel, MessageBoxImage.Information
                        );
                        if (mbr == MessageBoxResult.Cancel)
                            return;
                    }

                    List<PrepareMode> prpSuperlist = new List<PrepareMode>();
                    for (int i = 0; i < iFilesPrpA.Count; i++)
                    {
                        filePathA = Path.Combine(parentDirPathA, iFilesPrpA[i].NameExt);
                        filePathB = Path.Combine(parentDirPathB, iFilesPrpB[i].NameExt);
                        filePathC = Path.Combine(parentDirPathC, iFilesPrpC[i].NameExt);
                        PrepareMode p = new PrepareMode(filePathA, filePathB, filePathC, Tbx_Prp_TextToReplace.Text, Tbx_Prp_ReplacingText.Text)
                        {
                            Title = $"{processedFilesCount + 1}: {iFilesPrpA[i].Name}"
                        };
                        p.ProcessFiles(true);
                        prpSuperlist.Add(p);
                        processedFilesCount += 1;
                    }
                    processingMode = new ProcessingMode() { AllPrepareModeEntries = prpSuperlist };
                    outputPreview = new OutputPreview(processingMode);
                    outputPreview.ShowDialog();
                    break;
            }
        }

        #endregion


        #region Output

        private void Btn_GenerateOutput_Click(object sender, RoutedEventArgs e) // TODO: Factorize with Preview common elements
        {
            int processedFilesCount = 0;
            List<string> iFilenames;
            List<string> alreadyExisting;
            List<int> linesWithDistinctFilenames;
            string parentDirPathA;
            string parentDirPathB;
            string parentDirPathC;
            string fileFmgVersionA;
            string fileFmgVersionB;
            string fileFmgVersionC;
            string filePathA;
            string filePathB;
            string filePathC;

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
                        MessageBoxResult mbr = MessageBox.Show(
                            MSG_OVERW_EXIST_OF + $"\r\n\r\n - {fnames}", HDR_OVERW_EXIST_OF, MessageBoxButton.OKCancel, MessageBoxImage.Information
                        );
                        if (mbr == MessageBoxResult.Cancel)
                            return;
                    }

                    foreach (InputFile iFile in iFilesRd)
                    {
                        filePathA = Path.Combine(parentDirPathA, iFile.NameExt);
                        ReadMode r = new ReadMode(filePathA) { OneLinedValues = Cbx_Rd_OneLinedValues.IsChecked ?? false };
                        r.ProcessFiles(false);
                        r.ProduceOutput(iFile.Name + ".csv", Tbx_Rd_CsvSeparator.Text);
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
                        MessageBox.Show(MSG_WRONG_IFCOUNT, HDR_WRONG_IFCOUNT, MessageBoxButton.OK, MessageBoxImage.Error);
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
                    linesWithDistinctFilenames = CompareFilenames(iFilesCmpA, iFilesCmpB);
                    if (linesWithDistinctFilenames.Count != 0)
                    {   // Warning if filenames are not the same in all input areas
                        string lines = string.Join(", ", linesWithDistinctFilenames);
                        MessageBoxResult mbr = MessageBox.Show(
                            MSG_INCONS_IFNAMES1 + $"\r\n{lines}\r\n\r\n" + MSG_INCONS_IFNAMES2, HDR_INCONS_IFNAMES, MessageBoxButton.OKCancel, MessageBoxImage.Information
                        );
                        if (mbr == MessageBoxResult.Cancel)
                            return;
                    }

                    iFilenames = iFilesCmpA.Select(iFile => iFile.Name + ".csv").ToList();
                    alreadyExisting = Tools.GetAlreadyExistingFilenames(iFilenames);
                    if (alreadyExisting.Count > 0)
                    {
                        string fnames = string.Join("\r\n - ", alreadyExisting);
                        MessageBoxResult mbr = MessageBox.Show(
                            MSG_OVERW_EXIST_OF + $"\r\n\r\n - {fnames}", HDR_OVERW_EXIST_OF, MessageBoxButton.OKCancel, MessageBoxImage.Information
                        );
                        if (mbr == MessageBoxResult.Cancel)
                            return;
                    }

                    for (int i = 0; i < iFilesCmpA.Count; i++)
                    {
                        filePathA = Path.Combine(parentDirPathA, iFilesCmpA[i].NameExt);
                        filePathB = Path.Combine(parentDirPathB, iFilesCmpB[i].NameExt);
                        CompareMode c = new CompareMode(filePathA, filePathB) { OneLinedValues = Cbx_Cmp_OneLinedValues.IsChecked ?? false };
                        c.ProcessFiles(false);
                        c.ProduceOutput(iFilesCmpA[i].Name + ".csv", Tbx_Cmp_oHeader1.Text, Tbx_Cmp_oHeader2.Text, Tbx_Cmp_CsvSeparator.Text);
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
                        MessageBox.Show(MSG_WRONG_IFCOUNT, HDR_WRONG_IFCOUNT, MessageBoxButton.OK, MessageBoxImage.Error);
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
                    linesWithDistinctFilenames = CompareFilenames(iFilesPrpA, iFilesPrpB, iFilesPrpC);
                    if (linesWithDistinctFilenames.Count != 0)
                    {   // Warning if filenames are not the same in all input areas
                        string lines = string.Join(", ", linesWithDistinctFilenames);
                        MessageBoxResult mbr = MessageBox.Show(
                            MSG_INCONS_IFNAMES1 + $"\r\n{lines}\r\n\r\n" + MSG_INCONS_IFNAMES2, HDR_INCONS_IFNAMES, MessageBoxButton.OKCancel, MessageBoxImage.Information
                        );
                        if (mbr == MessageBoxResult.Cancel)
                            return;
                    }

                    iFilenames = iFilesPrpA.Select(iFile => iFile.NameExt).ToList();
                    alreadyExisting = Tools.GetAlreadyExistingFilenames(iFilenames);
                    if (alreadyExisting.Count > 0)
                    {
                        string fnames = string.Join("\r\n - ", alreadyExisting);
                        MessageBoxResult mbr = MessageBox.Show(
                            MSG_OVERW_EXIST_OF + $"\r\n\r\n - {fnames}", HDR_OVERW_EXIST_OF, MessageBoxButton.OKCancel, MessageBoxImage.Information
                        );
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
        }

        #endregion

    }
}
