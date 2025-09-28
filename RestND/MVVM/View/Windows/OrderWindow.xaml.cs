using RestND.MVVM.Model.Orders;
using RestND.MVVM.ViewModel.Orders;
using RestND.MVVM.Model.Orders;
using System.Windows;

namespace RestND.MVVM.View.Windows
{
    public partial class OrderWindow : Window
    {
        // keep for XAML designer / old calls
        public OrderWindow()
        {
            InitializeComponent();
            this.DataContext = new OrderViewModel();
        }

        public OrderWindow(Order order)
        { 
            InitializeComponent();
            this.DataContext = new OrderViewModel(order);
        }
    }
}