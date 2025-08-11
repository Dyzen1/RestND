//using CommunityToolkit.Mvvm.ComponentModel;
//using CommunityToolkit.Mvvm.Input;
//using RestND.Data;
//using RestND.MVVM.Model.Tables;
//using RestND.Validations;
//using System;
//using System.Collections.ObjectModel;
//using System.Linq;
//using System.Collections.Generic;

//namespace RestND.MVVM.ViewModel
//{
//    public partial class TableViewModel : ObservableObject
//    {
//        #region Services

//        private readonly TableServices _tableService;
//        private readonly OrderServices _orderService;

//        #endregion

//        #region Observable Properties

//        [ObservableProperty]
//        private ObservableCollection<Table> tables = new();

//        [ObservableProperty]
//        private Table selectedTable;

//        [ObservableProperty]
//        private Table newTable = new();

//        #endregion

//        #region Constructor

//        public TableViewModel()
//        {
//            _tableService = new TableServices();
//            _orderService = new OrderServices();
//            LoadTables();
//        }

//        #endregion

//        #region Load Tables

//        [RelayCommand]
//        private void LoadTables()
//        {
//            Tables.Clear();

//            // Initialize 25 empty table slots
//            for (int i = 1; i <= Table.MAX_TABLE_NUMBER; i++)
//            {
//                Tables.Add(new Table
//                {
//                    Table_Number = i,
//                    Is_Active = false,
//                    Table_Status = false,
//                    C = 0,
//                    R = 0
//                });
//            }

//            // Load real tables from DB
//            var dbTables = _tableService.GetAll();

//            foreach (var dbTable in dbTables)
//            {
//                var match = Tables.FirstOrDefault(t => t.Table_Number == dbTable.Table_Number);
//                if (match != null)
//                {
//                    match.Table_ID = dbTable.Table_ID;
//                    match.Is_Active = dbTable.Is_Active;
//                    match.Table_Status = dbTable.Table_Status;
//                    match.C = dbTable.C;
//                    match.R = dbTable.R;
//                }
//            }

//            // Load active orders and flag those tables
//            var orders = _orderService.GetAll();
//            foreach (var order in orders)
//            {
//                var match = Tables.FirstOrDefault(t => t.Table_Number == order.Table.Table_Number);
//                if (match != null)
//                {
//                    match.Table_Status = true;
//                }
//            }
//        }

//        #endregion

//        #region Add Table

//        [RelayCommand]
//        private void AddTable()
//        {
//            var validator = new TableValidator(NewTable);
//            if (!validator.ValidateAll(out var errors))
//                return;

//            NewTable.Is_Active = true;
//            NewTable.Table_Status = false;

//            bool success = _tableService.Add(NewTable);
//            if (success)
//            {
//                LoadTables();
//                NewTable = new Table();
//            }
//        }

//        #endregion

//        #region Update Table

//        [RelayCommand(CanExecute = nameof(CanModifyTable))]
//        private void UpdateTable()
//        {
//            if (SelectedTable == null) return;

//            var validator = new TableValidator(SelectedTable);
//            if (!validator.ValidateAll(out var errors))
//                return;

//            bool success = _tableService.Update(SelectedTable);
//            if (success)
//            {
//                LoadTables();
//            }
//        }

//        #endregion

//        #region Delete Table

//        [RelayCommand(CanExecute = nameof(CanModifyTable))]
//        private void DeleteTable()
//        {
//            if (SelectedTable == null) return;

//            bool success = _tableService.Delete(SelectedTable);
//            if (success)
//            {
//                Tables.Remove(SelectedTable);
//            }
//        }

//        #endregion

//        #region On Change

//        partial void OnSelectedTableChanged(Table value)
//        {
//            DeleteTableCommand.NotifyCanExecuteChanged();
//            UpdateTableCommand.NotifyCanExecuteChanged();
//            AddTableCommand.NotifyCanExecuteChanged();
//        }

//        #endregion

//        #region CanExecute Helpers

//        private bool CanModifyTable()
//        {
//            return SelectedTable != null;
//        }

//        #endregion
//    }
//}
