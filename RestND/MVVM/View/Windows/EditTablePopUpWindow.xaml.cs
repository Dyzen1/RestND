using RestND.MVVM.ViewModel.Main;
using System;
using System.Windows;

namespace RestND.MVVM.View.Windows
{
    public partial class EditTablePopUpWindow : Window
    {
        private readonly MainWindowViewModel _mainVM;

        public EditTablePopUpWindow(MainWindowViewModel mainVM)
        {
            InitializeComponent();

            _mainVM = mainVM;

            // Make sure ActiveTables is up to date
            _mainVM.LoadTables();

            // Hook close action (you already use ClosePopupAction in VM)
            _mainVM.ClosePopupAction = Close;
            DataContext = _mainVM;
        }

        // Optional: block wrong usage so this bug never comes back
        public EditTablePopUpWindow()
        {
            InitializeComponent();
            throw new InvalidOperationException("Use EditTablePopUpWindow(MainWindowViewModel mainVM)");
        }
    }
}
