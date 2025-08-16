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

namespace RestND.MVVM.View.Windows
{
    /// <summary>
    /// Interaction logic for EditEmployeeWindow.xaml
    /// </summary>
    public partial class EditEmployeeWindow : Window
    {
        public EditEmployeeWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        // <-- This fixes the missing handler error
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // optional, only if opened via ShowDialog()
            Close();
        }

        // Optional helper if you want to close after a successful add from the VM:
        public void CloseOnSuccess()
        {
            DialogResult = true;  // optional
            Close();
        }
    }
}
