using RestND.Data;
using RestND.MVVM.Model.Tables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.Validations
{
    public class TableValidator
    {

        private readonly Table _table;
        private readonly TableServices _tableService;

        public TableValidator(Table table)
        {
            _table = table;
            _tableService = new TableServices();
        }

        public bool ValidateTableNumber(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (_table.Table_Number <= 0)
            {
                errorMessage = "Table number must be greater than 0!";
                return false;
            }

            List<Table> existingTables = _tableService.GetAll();

            if (existingTables.Any(t => t.Table_Number == _table.Table_Number && t.Table_ID != _table.Table_ID))
            {
                errorMessage = "Table number already exists!";
                return false;
            }

            return true;
        }

        public bool ValidateCoordinates(out string errorMessage)
        {
            errorMessage = string.Empty;

            if (_table.X < 0 || _table.Y < 0)
            {
                errorMessage = "Coordinates must be positive values!";
                return false;
            }
            if (_table.X == _table.Y)
            {
                errorMessage = "Coordinates cannot be the same value!";
                return false;
            }


            if (_table.X > 1000 || _table.Y > 1000)
            {
                errorMessage = "Coordinates exceed layout boundaries!";
                return false;
            }

            return true;
        }

   
    }
}
