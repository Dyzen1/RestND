﻿using RestND.MVVM.ViewModel.Orders;
using System.Windows;
using System.Windows.Input;

namespace RestND.MVVM.View.Windows
{
    public partial class OverView : Window
    {
        public OverView()
        {
            InitializeComponent();
            this.DataContext = new OrderViewModel();
        }

        private void return_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = new MainWindow();
            mainWindow.WindowState = WindowState.Maximized;
            mainWindow.Show();
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        //close + minimize + maximize window.
        private void MinimizeBtn_Click(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void MaximizeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (WindowState == WindowState.Maximized)
                WindowState = WindowState.Normal;
            else
                WindowState = WindowState.Maximized;
        }

        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
