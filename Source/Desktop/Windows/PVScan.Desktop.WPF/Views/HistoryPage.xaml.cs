using PVScan.Desktop.WPF.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PVScan.Desktop.WPF.Views
{
    /// <summary>
    /// Interaction logic for HistoryPage.xaml
    /// </summary>
    public partial class HistoryPage : ContentControl
    {
        double SearchDelay = 500;
        Timer SearchDelayTimer;

        HistoryPageViewModel VM;

        public HistoryPage()
        {
            InitializeComponent();

            SearchDelayTimer = new Timer(SearchDelay);
            SearchDelayTimer.Enabled = false;
            SearchDelayTimer.Elapsed += SearchDelayTimer_Elapsed;
            SearchDelayTimer.AutoReset = false;

            VM = DataContext as HistoryPageViewModel;
        }

        private void SearchDelayTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                VM.SearchCommand.Execute(null);
            });
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SearchDelayTimer.Interval = SearchDelay;
            SearchDelayTimer.Enabled = true;
        }
    }
}
