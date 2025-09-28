using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR.Client;
using RestND.Data;
using RestND.MVVM.Model.Employees;
using RestND.MVVM.Model.Orders;
using RestND.MVVM.Model.Security;
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
        private readonly TableServices _tableService = new();
        private readonly TableValidator _validator = new();
        private readonly HubConnection _hub = App.MainHub;
        private readonly EmployeeServices _employeeServices = new();
        public Action? ClosePopupAction { get; set; }
        public event Action<Table>? OpenEmpForOrder;

        [ObservableProperty] private ObservableCollection<Table> tables = new();
        [ObservableProperty] private ObservableCollection<Table> activeTables = new();
        [ObservableProperty] private Table newTable = new();
        [ObservableProperty] private Table selectedTable;
        [ObservableProperty] private string editedTableNumberText;
        [ObservableProperty] private string newTableNumberText;
        [ObservableProperty] private string newTableMaxDinersText = "2";  // ONE definition only
        [ObservableProperty] private string tableErrorMessage;
        [ObservableProperty] private bool isLoggedIn;

        [ObservableProperty] private ObservableCollection<Employee> employees = new();
        [ObservableProperty] private Employee selectedEmployee;
        [ObservableProperty] private Table selectedTableForOrder;
        [ObservableProperty] private int dinersCount = 1;
        [ObservableProperty] private string orderPopupError;

        public string LoginButtonText => IsLoggedIn ? "Logout" : "Login";

        partial void OnIsLoggedInChanged(bool oldValue, bool newValue) =>
            OnPropertyChanged(nameof(LoginButtonText));

        partial void OnSelectedTableChanged(Table value)
        {
            AddTableCommand.NotifyCanExecuteChanged();
            EditTableCommand.NotifyCanExecuteChanged();
            DeleteTableCommand.NotifyCanExecuteChanged();
        }

        public MainWindowViewModel()
        {
            LoadTables();
            LoadEmployees();
            intiSR();
        }

        private void intiSR()
        {
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

        [RelayCommand]
        private void LoginLogout()
        {
            if (!IsLoggedIn)
            {
                var loginWindow = new View.Windows.LoginWindow();
                if (loginWindow.DataContext is Employees.LoginViewModel loginVm)
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
                AuthContext.SignOut();
                IsLoggedIn = false;
            }
        }

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
                    ActiveTables.Add(table);
            }
        }

        [RelayCommand]
        public async Task AddTable()
        {
            if (!_validator.IsEmptyField(NewTableNumberText, out string emptyErr))
            {
                TableErrorMessage = emptyErr; return;
            }
            if (!int.TryParse(NewTableNumberText, out int parsedNumber))
            {
                TableErrorMessage = "Please enter a valid number."; return;
            }
            if (!_validator.CheckPosNum(parsedNumber, out string notPositiveErr))
            {
                TableErrorMessage = notPositiveErr; return;
            }
            if (!_validator.CheckIfExists(parsedNumber, out string existsErr))
            {
                TableErrorMessage = existsErr; return;
            }
            if (!_validator.isFull(out string fullErr))
            {
                TableErrorMessage = fullErr; return;
            }
            if (!int.TryParse(NewTableMaxDinersText, out int parsedMax) || parsedMax <= 0)
            {
                TableErrorMessage = "Max diners must be a positive number."; return;
            }

            TableErrorMessage = string.Empty;

            var slot = Tables.FirstOrDefault(t => !t.Is_Active);
            if (slot == null) return;

            NewTable.Table_Number = parsedNumber;
            NewTable.Is_Active = true;
            NewTable.Table_Status = true;
            NewTable.C = slot.C;
            NewTable.R = slot.R;
            NewTable.Max_Diners = parsedMax;

            if (_tableService.Add(NewTable))
            {
                await _hub.SendAsync("NotifyTableUpdate", NewTable, "add");
                NewTable = new Table();
                NewTableNumberText = string.Empty;
                NewTableMaxDinersText = "2";
                LoadTables();
            }
        }

        [RelayCommand]
        public async Task DeleteTable()
        {
            TableErrorMessage = string.Empty;
            if (!_validator.CheckIfnull(SelectedTable, out string nullErr))
            { TableErrorMessage = nullErr; return; }

            if (_tableService.Delete(SelectedTable))
            {
                await _hub.SendAsync("NotifyTableUpdate", SelectedTable, "delete");
                LoadTables();
                ClosePopupAction?.Invoke();
            }
        }

        [RelayCommand]
        public async Task EditTable()
        {
            TableErrorMessage = string.Empty;

            if (!int.TryParse(EditedTableNumberText, out int parsedNumber))
            { TableErrorMessage = "Please enter a valid number."; return; }

            if (!_validator.CheckPosNum(parsedNumber, out string notPositiveErr))
            { TableErrorMessage = notPositiveErr; return; }

            var duplicate = Tables.FirstOrDefault(t =>
                t.Table_ID != SelectedTable?.Table_ID &&
                t.Table_Number == parsedNumber);

            if (duplicate != null)
            { TableErrorMessage = "Another table already has this number."; return; }

            if (!_validator.CheckIfnull(SelectedTable, out string nullErr))
            { TableErrorMessage = nullErr; return; }

            SelectedTable.Table_Number = parsedNumber;

            if (_tableService.Update(SelectedTable))
            {
                await _hub.SendAsync("NotifyTableUpdate", SelectedTable, "update");
                LoadTables();
                ClosePopupAction?.Invoke();
            }
        }

        public void LoadEmployees()
        {
            Employees.Clear();
            var waiters = _employeeServices.GetByRoleName("Waiter");
            foreach (var emp in waiters) Employees.Add(emp);
            if (Employees.Count > 0) SelectedEmployee = Employees[0];
        }

        [RelayCommand]
        private void TableClick(Table table)
        {
            SelectedTableForOrder = table;
            DinersCount = 1;
            OrderPopupError = string.Empty;
            OpenEmpForOrder?.Invoke(table);
        }

        [RelayCommand]
        private void CreateOrder()
        {
            OrderPopupError = string.Empty;

            if (SelectedTableForOrder == null)
            { OrderPopupError = "No table selected."; return; }
            if (SelectedEmployee == null)
            { OrderPopupError = "Please choose a waiter."; return; }
            if (DinersCount <= 0)
            { OrderPopupError = "Number of diners must be positive."; return; }
            if (SelectedTableForOrder.Max_Diners > 0 && DinersCount > SelectedTableForOrder.Max_Diners)
            { OrderPopupError = $"Table allows up to {SelectedTableForOrder.Max_Diners} diners."; return; }

            var order = new Order
            {
                assignedEmployee = SelectedEmployee,
                Table = SelectedTableForOrder,
                People_Count = DinersCount
            };

            new View.Windows.OrderWindow(order)
            {
                Owner = Application.Current.MainWindow,
                WindowStartupLocation = WindowStartupLocation.CenterOwner
            }.Show();

            ClosePopupAction?.Invoke();
        }
    }
}
