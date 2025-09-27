using Microsoft.Win32;
using RestND.MVVM.ViewModel.Windows;
using System;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace RestND.MVVM.View.Windows
{
    public partial class ReportWindow : Window
    {
        public ReportWindow()
        {
            InitializeComponent();
            DataContext = new ReportViewModel();
        }
        private void DownloadPng_Click(object sender, RoutedEventArgs e)
        {
            if (ReportChart == null) return;

            var sfd = new SaveFileDialog
            {
                Title = "Save Chart as PNG",
                Filter = "PNG Image (*.png)|*.png",
                FileName = $"DishSalesChart_{DateTime.Now:yyyyMMdd_HHmm}.png"
            };

            if (sfd.ShowDialog() != true) return;

            // Freeze interaction so templates/hover states don’t re-render mid-capture
            var wasEnabled = ReportChart.IsEnabled;
            ReportChart.IsEnabled = false;

            // Make sure layout is up-to-date, then capture using exact integer size
            ReportChart.UpdateLayout();
            int width = Math.Max(1, (int)Math.Round(ReportChart.ActualWidth));
            int height = Math.Max(1, (int)Math.Round(ReportChart.ActualHeight));

            // If the control is collapsed/min size, bail out
            if (width < 2 || height < 2)
            {
                ReportChart.IsEnabled = wasEnabled;
                MessageBox.Show("Chart area is too small to export.");
                return;
            }

            // Render the visual
            var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
            rtb.Render(ReportChart);

            // Encode and save using the dialog’s stream (avoids some path/permission quirks)
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(rtb));

            using (var stream = sfd.OpenFile()) // FileMode.Create by default
            {
                encoder.Save(stream);
            }

            ReportChart.IsEnabled = wasEnabled;
        }
    }
}
