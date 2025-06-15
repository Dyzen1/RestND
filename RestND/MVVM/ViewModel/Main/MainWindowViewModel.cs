using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;
using RestND.MVVM.Model.Tables;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace RestND.MVVM.ViewModel.Main
{
    public partial class MainWindowViewModel : ObservableObject
    {
        private readonly TableServices _tableService = new();
        private readonly HubConnection _hub = App.MainHub;

        [ObservableProperty]
        private ObservableCollection<Table> tables = new();

        [ObservableProperty]
        private Table newTable = new();

        public MainWindowViewModel()
        {
            LoadTables();

            _hub.On<Table, string>("ReceiveTableUpdate", (table, action) =>
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    var existing = Tables.FirstOrDefault(t => t.Table_ID == table.Table_ID);
                    switch (action)
                    {
                        case "add":
                            if (existing == null) Tables.Add(table);
                            break;
                        case "update":
                            if (existing != null)
                            {
                                existing.Table_Number = table.Table_Number;
                                existing.C = table.C;
                                existing.R = table.R;
                                existing.Table_Status = table.Table_Status;
                                existing.Is_Active = table.Is_Active;
                            }
                            break;
                        case "delete":
                            if (existing != null) Tables.Remove(existing);
                            break;
                    }
                });
            });
        }

        private void LoadTables()
        {
            var result = _tableService.GetAll();

            Tables.Clear();

            // Fill 25 slots
            for (int i = 0; i < 25; i++)
            {
                var t = result.FirstOrDefault(t => t.C == i % 5 && t.R == i / 5);
                Tables.Add(t ?? new Table
                {
                    Table_ID = -1,
                    Table_Number = 0,
                    C = i % 5,
                    R = i / 5,
                    Table_Status = false,
                    Is_Active = false
                });
            }

        }

        [RelayCommand]
        public async Task AddTableAsync()
        {
            var slot = Tables.FirstOrDefault(t => !t.Is_Active);
            if (slot == null)
            {
                MessageBox.Show("All 25 table slots are occupied.");
                return;
            }

            var doesExist = Tables.FirstOrDefault(t => t.Table_Number == NewTable.Table_Number );
            if (doesExist != null)
            {
                MessageBox.Show("Table number already exists");
                return;
            }
            NewTable.Is_Active = true;
            NewTable.Table_Status = true;
            NewTable.C = slot.C;
            NewTable.R = slot.R;

            if (_tableService.Add(NewTable))
            {
                await _hub.SendAsync("NotifyTableUpdate", NewTable, "add");
                NewTable = new Table();
                LoadTables();
            }
        }

        [RelayCommand]
        public async Task DeleteTableAsync(Table table)
        {
            if (_tableService.Delete(table))
            {
                await _hub.SendAsync("NotifyTableUpdate", table, "delete");
            }
        }

        [RelayCommand]
        public async Task EditTableAsync(Table table)
        {
            if (_tableService.Update(table))
            {
                await _hub.SendAsync("NotifyTableUpdate", table, "update");
            }
        }
        [RelayCommand]
        private void TableClick(Table table)
        {
            if (!table.Is_Active)
            {
                MessageBox.Show("This table is inactive.");
                return;
            }

            MessageBox.Show($"Opening Table #{table.Table_Number}");
            // TODO: Navigate to order screen
        }
    }
}
