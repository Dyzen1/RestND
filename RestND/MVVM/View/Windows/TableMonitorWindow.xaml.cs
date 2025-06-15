using RestND.MVVM.ViewModel.Main;
using System.Windows;

namespace RestND.MVVM.View.Windows
{
    public partial class TableMonitorWindow : Window
    {
        public TableMonitorWindow()
        {
            InitializeComponent();
            DataContext = App.SharedMainVM;
        }
    }
}
