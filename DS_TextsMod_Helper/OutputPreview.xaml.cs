using System.Windows;
using System.Windows.Controls;

namespace DS_TextsMod_Helper
{
    public partial class OutputPreview : Window
    {
        public OutputPreview(ProcessingMode processingMode)
        {
            InitializeComponent();

            if (!(processingMode.AllReadModeEntries is null))
            {
                Title = "Output preview : Read mode";
                Cbx_HighlightItems.Visibility = Visibility.Collapsed;
                Tbc_Preview_Read.Visibility = Visibility.Visible;
                Tbc_Preview_Read.ItemsSource = processingMode.AllReadModeEntries;
            }
            if (!(processingMode.AllCompareModeEntries is null))
            {
                Title = "Output preview : Compare mode";
                Cbx_HighlightItems.Content = "Highlight contents (same / different)";
                Tbc_Preview_Compare.Visibility = Visibility.Visible;
                Tbc_Preview_Compare.ItemsSource = processingMode.AllCompareModeEntries;
            }
            if (!(processingMode.AllPrepareModeEntries is null))
            {
                Title = "Output preview : Prepare mode";
                Cbx_HighlightItems.Content = "Highlight contents (translated / not translated)";
                Tbc_Preview_Prepare.Visibility = Visibility.Visible;
                Tbc_Preview_Prepare.ItemsSource = processingMode.AllPrepareModeEntries;
            }
        }

        private void Dtg_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
    }
}
