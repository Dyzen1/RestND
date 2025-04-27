using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model.Tables;
using System.Collections.ObjectModel;

namespace RestND.MVVM.ViewModel{

public partial class TableViewModel : ObservableObject{
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

    #region LoadTables

        [RelayCommand]
        private void LoadTables()
        {
            tables.Clear();
            var dbTables = _tableService.GetAll();
            foreach (var table in dbTables)
                tables.Add(table);
        }
        #endregion
    
    #region AddTable
        [RelayCommand]
        private void AddTable()
        {
            if(!string.IsNullOrWhiteSpace(newTable.Table_Number.ToString()) && newTable.Table_Number >= 0)
            {
                bool success = _tableService.Add(newTable);

                if(success){
                    LoadTables();
                    newTable = new Table();
                }
            }
        }
        #endregion

    #region DeleteTable

        [RelayCommand(CanExecute = nameof(CanModifyProduct))]
        private void DeleteTable()
        {
            if(selectedTable != null)
            {
                bool success = _tableService.Delete(selectedTable.Table_ID);
                if(success)
                {
                    tables.Remove(selectedTable);
                }
            }
        }
        #endregion

    #region UpdateTable


        [RelayCommand(CanExecute = nameof(CanModifyProduct))]
        private void UpdateTable()
        {
            if(selectedTable != null)
            {
                bool success = _tableService.Update(selectedTable);
                if(success)
                {
                    LoadTables();
                }
            }
        }
        #endregion

    #region CanExecute Helpers

        private bool CanModifyProduct()
        {
            return selectedTable != null;
        }

        #endregion


    }




}