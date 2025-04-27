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
    /// <summary>
    /// Interaction logic for ProductWindow.xaml
    /// </summary>
    public partial class ProductWindow : Window
    {
        public ProductWindow()
        {
            InitializeComponent();
            this.DataContext = new ProductViewModel();
        }

        private void return_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            {
                WindowState = WindowState.Maximized;
            }
            mainWindow.ShowDialog();
            this.Close();
        }
    }
}
