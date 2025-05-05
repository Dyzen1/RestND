using RestND.MVVM.ViewModel;
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

namespace RestND.MVVM.View
{
    public partial class DishWindow : Window
    {
        public DishWindow()
        {
            InitializeComponent();
            this.DataContext = new DishViewModel();
        }

        private void return_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            {
                mainWindow.WindowState = WindowState.Maximized;
            }
            mainWindow.ShowDialog();
            this.Close();
        }
    }
}
