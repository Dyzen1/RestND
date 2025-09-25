using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using LiveCharts;
using LiveCharts.Wpf;
using RestND.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Microsoft.Win32;

namespace RestND.MVVM.ViewModel.Windows
{
    public partial class ReportViewModel : ObservableObject
    {
        private readonly ReportServices _reportService = new();

        [ObservableProperty] private SeriesCollection seriesCollection;
        [ObservableProperty] private List<string> labels;

        [ObservableProperty] private DateTime fromDate = DateTime.Now.AddDays(-7);
        [ObservableProperty] private DateTime toDate = DateTime.Now;

        [ObservableProperty] private string selectedFilter = "Best Selling";

        // Y-axis integer formatting
        [ObservableProperty] private Func<double, string> yFormatter = v => v.ToString("0");

        // Keep last loaded rows for export
        [ObservableProperty] private ObservableCollection<(string DishName, int QuantitySold)> currentReport = new();

        public List<string> FilterOptions { get; } = new() { "Best Selling", "Least Selling" };

        public ReportViewModel()
        {
            LoadChartData();
        }

        [RelayCommand]
        private void LoadChartData()
        {
            var inclusiveTo = ToDate.Date.AddDays(1).AddTicks(-1);
            var dishSales = _reportService.GetDishSales(FromDate.Date, inclusiveTo);

            dishSales = (SelectedFilter == "Best Selling")
                ? dishSales.OrderByDescending(d => d.QuantitySold).Take(5).ToList()
                : dishSales.OrderBy(d => d.QuantitySold).Take(5).ToList();

            CurrentReport = new ObservableCollection<(string DishName, int QuantitySold)>(dishSales);

            SeriesCollection = new SeriesCollection
            {
                new ColumnSeries
                {
                    Title = "Sales",
                    Values = new ChartValues<int>(dishSales.Select(d => d.QuantitySold))
                }
            };

            Labels = dishSales.Select(d => d.DishName).ToList();
        }

        [RelayCommand]
        private void ExportCsv()
        {
            try
            {
                if (CurrentReport == null || CurrentReport.Count == 0)
                {
                    MessageBox.Show("No data to export.");
                    return;
                }

                var sfd = new SaveFileDialog
                {
                    Title = "Save Report",
                    Filter = "CSV file (*.csv)|*.csv",
                    FileName = $"DishSales_{FromDate:yyyyMMdd}_{ToDate:yyyyMMdd}.csv"
                };

                if (sfd.ShowDialog() == true)
                {
                    using var sw = new StreamWriter(sfd.FileName);
                    sw.WriteLine("Dish Name,Quantity Sold");
                    foreach (var row in CurrentReport)
                        sw.WriteLine($"\"{row.DishName}\",{row.QuantitySold}");

                    MessageBox.Show("CSV exported successfully!");
                }
                else
                {
                    MessageBox.Show("Export canceled.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Export failed: {ex.Message}");
            }
        }

    }
}
