// RestND/MVVM/View/Windows/OthersWindow.xaml.cs
using RestND.MVVM.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace RestND.MVVM.View.Windows
{
    public partial class OthersWindow : Window
    {
        private readonly RoleViewModel _rolesVM = new RoleViewModel();
        public OthersWindow()
        {
            InitializeComponent();
            RolesSection.DataContext = _rolesVM;
        }

        // Window drag
        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        // Shared expander stub
        private void Expander_Expanded(object sender, RoutedEventArgs e)
        {
            // optional: collapse others, or do nothing
        }

        // VAT stubs
        private void OpenVat_Click(object sender, RoutedEventArgs e) { }
        private void AddVat_Click(object sender, RoutedEventArgs e) { }
        private void EditVat_Click(object sender, RoutedEventArgs e) { }
        private void DeleteVat_Click(object sender, RoutedEventArgs e) { }

        // Discount stubs
        private void AddDiscount_Click(object sender, RoutedEventArgs e) { }
        private void EditDiscount_Click(object sender, RoutedEventArgs e) { }
        private void DeleteDiscount_Click(object sender, RoutedEventArgs e) { }

        // DishType stubs
        private void AddDishType_Click(object sender, RoutedEventArgs e) { }
        private void EditDishType_Click(object sender, RoutedEventArgs e) { }
        private void DeleteDishType_Click(object sender, RoutedEventArgs e) { }
    }
}
