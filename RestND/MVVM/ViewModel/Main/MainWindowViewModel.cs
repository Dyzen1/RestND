using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;
using RestND.MVVM.Model.Tables;
using RestND.Validations;
using System;
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
        private readonly TableValidator _validator = new();
        private readonly HubConnection _hub = App.MainHub;
        public Action? ClosePopupAction { get; set; }

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
        private string editedTableNumberText;

        [ObservableProperty]
        private string newTableNumberText;

        [ObservableProperty]
        private string tableErrorMessage;

        [ObservableProperty]
        private bool isLoggedIn;

        public string LoginButtonText => IsLoggedIn ? "Logout" : "Login";

        partial void OnIsLoggedInChanged(bool oldValue, bool newValue)
        {
            OnPropertyChanged(nameof(LoginButtonText));
        }

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

        #region On Change
        partial void OnSelectedTableChanged(Table value)
        {
            AddTableCommand.NotifyCanExecuteChanged();
            EditTableCommand.NotifyCanExecuteChanged();
            DeleteTableCommand.NotifyCanExecuteChanged();
        }
        #endregion

        [RelayCommand]
        private void LoginLogout()
        {
            if (!IsLoggedIn)
            {
                // Show Login Window
                var loginWindow = new RestND.MVVM.View.Windows.LoginWindow();

                // Setup ViewModel event to get login success callback
                if (loginWindow.DataContext is RestND.MVVM.ViewModel.Employees.LoginViewModel loginVm)
                {
                    loginVm.LoginSucceeded += () =>
                    {
                        IsLoggedIn = true;
                        loginWindow.Close();
                    };
                }

                loginWindow.ShowDialog();
            }
            else
            {
                // Logout logic
                IsLoggedIn = false;
                // You can add any cleanup/reset here
            }
        }

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
        public async Task AddTable()
        {
            // 1. check if input is empty
            if(!_validator.IsEmptyField(NewTableNumberText, out string emptyErr))
            {
                TableErrorMessage = emptyErr;
                return;
            }
            // 2. Parse input
            if (!int.TryParse(NewTableNumberText, out int parsedNumber))
            {
                TableErrorMessage = "Please enter a valid number.";
                return;
            }
            // 3. Validate it's positive
            if (!_validator.CheckPosNum(parsedNumber, out string notPositiveErr))
            {
                TableErrorMessage = notPositiveErr;
                return;
            }
            // 4. Validate table number uniqueness
            if (!_validator.CheckIfExists(parsedNumber, out string existsErr))
            {
                TableErrorMessage = existsErr;
                return;
            }
            // 5. Check if there’s room for a new table
            if (!_validator.isFull(out string fullErr))
            {
                TableErrorMessage = fullErr;
                return;
            }
            // 6. Validation passed
            TableErrorMessage = string.Empty;

            var slot = Tables.FirstOrDefault(t => !t.Is_Active);
            if (slot == null) return;

            NewTable.Table_Number = parsedNumber;
            NewTable.Is_Active = true;
            NewTable.Table_Status = true;
            NewTable.C = slot.C;
            NewTable.R = slot.R;

            if (_tableService.Add(NewTable))
            {
                await _hub.SendAsync("NotifyTableUpdate", NewTable, "add");
                NewTable = new Table();
                NewTableNumberText = string.Empty;
                LoadTables();
                //ClosePopupAction?.Invoke();
            }
        }
        #endregion

        #region Delete Table

        [RelayCommand]
        public async Task DeleteTable()
        {
            TableErrorMessage = string.Empty;

            // 1. Validate that a table is selected
            if (!_validator.CheckIfnull(SelectedTable, out string nullErr))
            {
                TableErrorMessage = nullErr;
                return;
            }

            // 2. Proceed with delete
            if (_tableService.Delete(SelectedTable))
            {
                await _hub.SendAsync("NotifyTableUpdate", SelectedTable, "delete");
                LoadTables();
                ClosePopupAction?.Invoke(); // Optional: close popup
            }
        }

        #endregion

        #region Edit Table

        [RelayCommand]
        public async Task EditTable()
        {
            TableErrorMessage = string.Empty;

            // 1. Validate user input is a number
            if (!int.TryParse(EditedTableNumberText, out int parsedNumber))
            {
                TableErrorMessage = "Please enter a valid number.";
                return;
            }

            // 2. Validate it's a positive number
            if (!_validator.CheckPosNum(parsedNumber, out string notPositiveErr))
            {
                TableErrorMessage = notPositiveErr;
                return;
            }

            // 3. Check if another table has this number
            var duplicate = Tables.FirstOrDefault(t =>
                t.Table_ID != SelectedTable?.Table_ID &&
                t.Table_Number == parsedNumber);

            if (duplicate != null)
            {
                TableErrorMessage = "Another table already has this number.";
                return;
            }

            // 4. Check if a table is selected
            if (!_validator.CheckIfnull(SelectedTable, out string nullErr))
            {
                TableErrorMessage = nullErr;
                return;
            }

            // 5. Apply and save
            SelectedTable.Table_Number = parsedNumber;

            if (_tableService.Update(SelectedTable))
            {
                await _hub.SendAsync("NotifyTableUpdate", SelectedTable, "update");
                LoadTables();
                ClosePopupAction?.Invoke(); // Optional: close the popup after success
            }
        }

        #endregion

        #region Table Click

        [RelayCommand]
        private void TableClick(Table table)
        {
            if (!_validator.CheckIfnull(table, out string msg))
            {
                TableErrorMessage = msg;
                return;
            }

            if (!table.Is_Active)
            {
                TableErrorMessage = "This table is inactive.";
                return;
            }

            TableErrorMessage = string.Empty;

            // TODO: Navigate to order screen
        }

        #endregion
    }
}
