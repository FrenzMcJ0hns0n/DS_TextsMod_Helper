using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DS_TextsMod_Helper
{
    /// <summary>
    /// Logique d'interaction pour OutputPreview.xaml
    /// </summary>
    public partial class OutputPreview : Window
    {
        public OutputPreview(ProcessingMode processingMode)
        {
            InitializeComponent();

            if (!(processingMode.AllReadModeEntries is null))
            {
                Tbc_Preview_Read.Visibility = Visibility.Visible;
                Tbc_Preview_Read.ItemsSource = processingMode.AllReadModeEntries;
            }

            // if (!(processingMode.AllCompareEntries is null)) // Handle preview of CompareMode.Entry

            // if (!(processingMode.AllPrepareEntries is null)) // Handle preview of PrepareMode.Entry

        }
    }
}
