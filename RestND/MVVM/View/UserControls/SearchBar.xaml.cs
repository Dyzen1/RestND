using System;
using System.Windows;
using System.Windows.Controls;

namespace RestND.MVVM.View.UserControls
{
    public partial class SearchBar : UserControl
    {
        public event EventHandler<string> SearchTextChanged;

        public SearchBar()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register(nameof(SearchText), typeof(string), typeof(SearchBar),
                new PropertyMetadata(string.Empty));

        public string SearchText
        {
            get => (string)GetValue(SearchTextProperty);
            set => SetValue(SearchTextProperty, value);
        }


        // search bar placeholder
        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.Register(nameof(Placeholder), typeof(string), typeof(SearchBar),
                new PropertyMetadata("Search…"));
        public string Placeholder
        {
            get => (string)GetValue(PlaceholderProperty);
            set => SetValue(PlaceholderProperty, value);
        }


        private void PART_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var text = (sender as TextBox)?.Text ?? string.Empty;
            SearchTextChanged?.Invoke(this, text);
        }
    }
}
