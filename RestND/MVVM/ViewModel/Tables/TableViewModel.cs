using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model.Tables;
using System.Collections.ObjectModel;

namespace RestND.MVVM.ViewModel{

    public partial class TableViewModel : ObservableObject
    {
        #region Services
        private readonly TableServices _tableService;
        #endregion

        #region Fields
    
        [ObservableProperty]
        public ObservableCollection<Table> tables = new ObservableCollection<Table>();

        [ObservableProperty]
        public Table selectedTable;

        [ObservableProperty]
        private Table newTable = new Table();

        #endregion

        #region On Change
        partial void OnSelectedTableChanged(Table value)
        {
            DeleteTableCommand.NotifyCanExecuteChanged();
            UpdateTableCommand.NotifyCanExecuteChanged();
        }

        #endregion

        #region Constructor
        public TableViewModel()
        {
            _tableService = new TableServices();
            LoadTables();
        }

        #endregion

        #region Load Tables

        [RelayCommand]
        private void LoadTables()
        {
            Tables.Clear();
            var dbTables = _tableService.GetAll();
            foreach (var table in dbTables)
                Tables.Add(table);
        }

        #endregion
    
        #region AddTable

        [RelayCommand]
        private void AddTable()
        {
            if (string.IsNullOrWhiteSpace(NewTable.Table_Number.ToString()) || NewTable.Table_Number < 0) return;
            var success = _tableService.Add(NewTable);

            if (!success) return;
            LoadTables();
            NewTable = new Table();
        }

        #endregion

        #region Delete Table

        [RelayCommand(CanExecute = nameof(CanModifyProduct))]
        private void DeleteTable()
        {
            if(SelectedTable != null)
            {
                bool success = _tableService.Delete(SelectedTable.Table_ID);
                if(success)
                {
                    Tables.Remove(SelectedTable);
                }
            }
        }

        #endregion

        #region Update Table

        [RelayCommand(CanExecute = nameof(CanModifyProduct))]
        private void UpdateTable()
        {
            if (SelectedTable == null) return;
            bool success = _tableService.Update(SelectedTable);
            if (!success) return;
            LoadTables();
        }

        #endregion

        #region CanExecute Helpers
        private bool CanModifyProduct()
        {
            return SelectedTable != null;
        }

        #endregion


    }




}