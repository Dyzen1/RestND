using RestND.MVVM.Model;
using System;
using System.Collections;
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
    public partial class DishTypesSideBar : UserControl
    {
        public DishTypesSideBar()
        {
            InitializeComponent();
            List.SelectionChanged += (_, __) =>
            {
                if (SelectedDishType != null && SelectCommand?.CanExecute(SelectedDishType) == true)
                    SelectCommand.Execute(SelectedDishType);
            };
        }

        // ItemsSource
        public IEnumerable DishTypes
        {
            get => (IEnumerable)GetValue(DishTypesProperty);
            set => SetValue(DishTypesProperty, value);
        }
        public static readonly DependencyProperty DishTypesProperty =
            DependencyProperty.Register(nameof(DishTypes), typeof(IEnumerable), typeof(DishTypesSideBar));

        // Selected item (two-way)
        public DishType? SelectedDishType
        {
            get => (DishType?)GetValue(SelectedDishTypeProperty);
            set => SetValue(SelectedDishTypeProperty, value);
        }
        public static readonly DependencyProperty SelectedDishTypeProperty =
            DependencyProperty.Register(nameof(SelectedDishType), typeof(DishType), typeof(DishTypesSideBar), new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        // Command to run when selection changes (parameter = DishType)
        public ICommand? SelectCommand
        {
            get => (ICommand?)GetValue(SelectCommandProperty);
            set => SetValue(SelectCommandProperty, value);
        }
        public static readonly DependencyProperty SelectCommandProperty =
            DependencyProperty.Register(nameof(SelectCommand), typeof(ICommand), typeof(DishTypesSideBar));
    }
}
