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
        #region Services and Fields

        private readonly TableServices _tableService = new();
        private readonly HubConnection _hub = App.MainHub;

        #endregion

        #region Observable Properties

        [ObservableProperty]
        private ObservableCollection<Table> tables = new();

        [ObservableProperty]
        private ObservableCollection<Table> activeTables = new();

        [ObservableProperty]
        private Table newTable = new();

        [ObservableProperty]
        private Table selectedTable;

        [ObservableProperty]
        private int editedTableNumber;

        #endregion

        #region Constructor

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
                            if (existing != null)
                            {
                                Tables.Remove(existing);
                                ActiveTables.Remove(existing);
                            }
                            break;
                    }
                });
            });
        }

        #endregion

        #region Load Tables

        public void LoadTables()
        {
            var result = _tableService.GetAll();

            Tables.Clear();
            ActiveTables.Clear();

            for (int i = 0; i < 25; i++)
            {
                var t = result.FirstOrDefault(t => t.C == i % 5 && t.R == i / 5);
                var table = t ?? new Table
                {
                    Table_ID = -1,
                    Table_Number = 0,
                    C = i % 5,
                    R = i / 5,
                    Table_Status = false,
                    Is_Active = false
                };

                Tables.Add(table);
                if (table.Is_Active)
                {
                    ActiveTables.Add(table);
                }
            }
        }

        #endregion

        #region Add Table

        [RelayCommand]
        public async Task AddTableAsync()
        {
            var slot = Tables.FirstOrDefault(t => !t.Is_Active);
            if (slot == null)
            {
                MessageBox.Show("All 25 table slots are occupied.");
                return;
            }

            var doesExist = Tables.FirstOrDefault(t => t.Table_Number == NewTable.Table_Number);
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

        #endregion

        #region Delete Table

        [RelayCommand]
        public async Task DeleteTableAsync()
        {
            if (_tableService.Delete(SelectedTable))
            {
                await _hub.SendAsync("NotifyTableUpdate", SelectedTable, "delete");
                LoadTables();
            }
        }

        #endregion

        #region Edit Table

        [RelayCommand]
        public async Task EditTableAsync()
        {
            var duplicate = Tables.FirstOrDefault(t =>
                t.Table_ID != SelectedTable.Table_ID &&
                t.Table_Number == EditedTableNumber);

            if (duplicate != null)
            {
                MessageBox.Show("Another table already has this number.");
                return;
            }

            SelectedTable.Table_Number = EditedTableNumber;

            if (_tableService.Update(SelectedTable))
            {
                await _hub.SendAsync("NotifyTableUpdate", SelectedTable, "update");
                LoadTables();
            }
        }

        #endregion

        #region Table Click

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

        #endregion
    }
}
