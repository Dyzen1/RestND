using System;
using System.Windows;
using System.Windows.Controls;

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

        private void OverViewBtn_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke("OverView");
        }

        private void DishesBtn_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke("Dishes");
        }

        private void ReportsBtn_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke("Reports");
        }

        private void EmployeesBtn_Click(object sender, RoutedEventArgs e)
        {
            ButtonClicked?.Invoke("Employees");
        }
    }
}
