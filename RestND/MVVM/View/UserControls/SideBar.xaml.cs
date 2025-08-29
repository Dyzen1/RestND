    using RestND.Data;
using RestND.MVVM.ViewModel.Navigation;
using System;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Navigation;

    namespace RestND.MVVM.View.UserControls
    {
        public partial class SideBar : UserControl
        {
            public SideBar()
            {
                InitializeComponent();
                this.DataContext = new NavigationViewModel();
            }

        }
    }
