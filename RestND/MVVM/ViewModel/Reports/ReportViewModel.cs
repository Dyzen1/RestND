using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveCharts;
using LiveCharts.Wpf;
using RestND.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestND.MVVM.ViewModel.Windows
{
    public partial class ReportViewModel : ObservableObject
    {
        private readonly ReportServices _reportService = new();

        [ObservableProperty]
        private SeriesCollection seriesCollection;

        [ObservableProperty]
        private List<string> labels;

        [ObservableProperty]
        private DateTime fromDate = DateTime.Now.AddDays(-7);

        [ObservableProperty]
        private DateTime toDate = DateTime.Now;

        [ObservableProperty]
        private string selectedFilter = "Best Selling";

        public List<string> FilterOptions { get; } = new() { "Best Selling", "Least Selling" };

        public ReportViewModel()
        {
            LoadChartData();
        }

        [RelayCommand]
        private void LoadChartData()
        {
            var dishSales = _reportService.GetDishSales(FromDate, ToDate);

            if (SelectedFilter == "Best Selling")
                dishSales = dishSales.OrderByDescending(d => d.QuantitySold).Take(5).ToList();
            else
                dishSales = dishSales.OrderBy(d => d.QuantitySold).Take(5).ToList();

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
    }
}
