//using RestND.Data;
//using RestND.MVVM.Model.Tables;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using RestND.MVVM.Model.Tables;

//namespace RestND.Validations
//{

//    public class TableValidator
//    {

//        private readonly Table _table;
//        private readonly TableServices _tableService;

//        public TableValidator(Table table)
//        {
//            _table = table;
//            _tableService = new TableServices();
//        }

//        public bool ValidateTableNumber(out string errorMessage)
//        {
//            errorMessage = string.Empty;

//            if (_table.Table_Number <= 0)
//            {
//                errorMessage = "Table number must be greater than 0!";
//                return false;
//            }

//            List<Table> existingTables = _tableService.GetAll();

//            if (existingTables.Any(t => t.Table_Number == _table.Table_Number && t.Table_ID != _table.Table_ID))
//            {
//                errorMessage = "Table number already exists!";
//                return false;
//            }

//            return true;
//        }

//        public bool ValidateCoordinates(out string errorMessage)
//        {
//            errorMessage = string.Empty;

//            if (_table.X < 0 || _table.Y < 0)
//            {
//                errorMessage = "Coordinates must be positive values!";
//                return false;
//            }

//            if (_table.X == _table.Y)
//            {
//                errorMessage = "Coordinates cannot be the same value!";
//                return false;
//            }


//            if (_table.X > 1000 || _table.Y > 1000)
//            {
//                errorMessage = "Coordinates exceed layout boundaries!";
//                return false;
//            }

//            return true;
//        }



//        public static class TableValidator
//        {
//            // Method to validate the table fields
//            public static Dictionary<string, List<string>> ValidateFields(Table table, List<Table> existingTables)
//            {
//                var errors = new Dictionary<string, List<string>>();

//                // Validate Table Number
//                if (table.Table_Number <= 0)
//                {
//                    AddError(errors, nameof(table.Table_Number), "Table number must be greater than zero.");
//                }
//                else if (existingTables.Any(t => t.Table_Number == table.Table_Number))
//                {
//                    AddError(errors, nameof(table.Table_Number), "Table number already exists!");
//                }

//                // Validate Coordinates (X and Y)
//                if (table.X == 0 && table.Y == 0)
//                {
//                    AddError(errors, nameof(table.X), "Table coordinates cannot be (0, 0).");
//                }

//                // Table Status Validation (optional if you want to validate it further)
//                if (table.Table_Status != true && table.Table_Status != false)
//                {
//                    AddError(errors, nameof(table.Table_Status), "Table status is invalid.");
//                }

//                return errors;
//            }

//            // Helper method to add errors to the dictionary
//            private static void AddError(Dictionary<string, List<string>> dict, string key, string message)
//            {
//                if (!dict.ContainsKey(key))
//                    dict[key] = new List<string>();
//                dict[key].Add(message);
//            }

//        }
//    }
//}
