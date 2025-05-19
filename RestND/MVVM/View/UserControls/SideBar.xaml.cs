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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace RestND.MVVM.View.UserControls
{
    public partial class SideBar : UserControl
    {
        public event Action<string> ButtonClicked;
        public SideBar()
        {
            InitializeComponent();
        }

        private void InventoryBtn_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke("Inventory");
        }

        private void OrdersBtn_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke("Orders");
        }


        private void DishesBtn_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke("Dishes");
        }

        private void ReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke("Reports");
        }

        //private void EmployeesBtn_Click(object sender, RoutedEventArgs e)
        //{
        //    ButtonClicked?.Invoke();
        //}
    }
}
