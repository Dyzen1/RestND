using RestND.MVVM.Model.Tables;
using RestND.Data;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace RestND.Validations
{
    public class TableValidator : GeneralValidations
    {
        private readonly TableServices _tableService = new();

        public bool CheckIfnull(Table table, out string err)
        {
            err = string.Empty;
            if (table == null)
            {
                err = "You must choose a table";
                return false;
            }
            return true;
        }

        public bool isFull(out string err)
        {
            err = string.Empty;
            List<Table> tables = _tableService.GetAll(); // returns only Is_Active == true

            if (tables.Count >= 25)
            {
                err = "All 25 table slots are occupied.";
                return false;
            }

            return true;
        }

        public bool CheckIfExists(int tableNumber, out string err)
        {
            err = string.Empty;
            List<Table> tables = _tableService.GetAll();
            var doesExist = tables.FirstOrDefault(t => t.Table_Number == tableNumber);
            if (doesExist != null)
            {
                err = "Table number already exists";
                return false;
            }
            return true;
        }
    }
}
