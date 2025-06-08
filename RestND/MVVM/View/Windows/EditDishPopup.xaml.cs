using System.Windows;

namespace RestND.MVVM.View.Windows
{
    public partial class EditDishPopup : Window
    {
        public EditDishPopup()
        {
            InitializeComponent();

            Loaded += (_, _) => Owner.Opacity = 0.4;
            Closed += (_, _) => Owner.Opacity = 1.0;
        }
    }
}
