using RestND.Data;
using RestND.MVVM.Model.Tables;
using System.Collections.Generic;
using System.Linq;

namespace RestND.Validations
{
    public class TableValidator
    {
        private readonly Table _table;
        private readonly List<Table> _existingTables;

        public TableValidator(Table table)
        {
            _table = table;
            _existingTables = new TableServices().GetAll();
        }

        public bool ValidateAll(out Dictionary<string, List<string>> errors)
        {
            errors = new Dictionary<string, List<string>>();

            ValidateTableNumber(errors);
            ValidateCoordinates(errors);

            return errors.Count == 0;
        }

        private void ValidateTableNumber(Dictionary<string, List<string>> errors)
        {
            if (_table.Table_Number <= 0)
            {
                AddError(errors, nameof(_table.Table_Number), "Table number must be greater than zero.");
            }
            else if (_existingTables.Any(t =>
                         t.Table_Number == _table.Table_Number &&
                         t.Is_Active &&
                         t.Table_ID != _table.Table_ID))
            {
                AddError(errors, nameof(_table.Table_Number), "Table number already exists and is active.");
            }
        }

        private void ValidateCoordinates(Dictionary<string, List<string>> errors)
        {
            if (_table.C < 0 || _table.R < 0)
                AddError(errors, "Coordinates", "Coordinates must be positive values.");

            if (_table.C == _table.R)
                AddError(errors, "Coordinates", "Coordinates cannot be equal.");

            if (_table.C > 1000 || _table.R > 1000)
                AddError(errors, "Coordinates", "Coordinates exceed layout boundaries.");
        }

        private void AddError(Dictionary<string, List<string>> dict, string key, string message)
        {
            if (!dict.ContainsKey(key))
                dict[key] = new List<string>();
            dict[key].Add(message);
        }
    }
}
