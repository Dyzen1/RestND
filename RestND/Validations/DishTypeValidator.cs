using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.Validations
{
    public class DishTypeValidator : GeneralValidations
    {
        private readonly DishTypeServices _dishTypeService = new();

        public bool CheckIfNull(DishType d, out string err)
        {
            err = string.Empty;
            if (d == null)
            {
                err = "You must choose/create a dish type.";
                return false;
            }
            return true;
        }

        public bool ValidName(
            string name,
            out string err,
            IEnumerable<DishType>? existing = null,
            bool checkExists = true,
            int? excludeId = null)
        {
            err = string.Empty;
            existing = _dishTypeService.GetAll();

            // Not empty
            if (!IsEmptyField(name, out var emptyErr))
            {
                err = emptyErr;
                return false;
            }

            // Format
            if (!isNameValid(name, out var formatErr))
            {
                err = formatErr;
                return false;
            }

            // Uniqueness
            if (checkExists && existing != null)
            {
                var exists = existing.Any(d =>
                    d.Is_Active &&
                    d.DishType_Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                    (!excludeId.HasValue || d.DishType_ID != excludeId.Value));

                if (exists)
                {
                    err = "Dish type with this name already exists!";
                    return false;
                }
            }

            return true;
        }

        public bool ExistsById(int typeId, IEnumerable<DishType> existingTypes, out string err)
        {
            err = string.Empty;
            existingTypes = _dishTypeService.GetAll();
            if (existingTypes == null || !existingTypes.Any(d => d.DishType_ID == typeId && d.Is_Active))
            {
                err = "Dish type does not exist.";
                return false;
            }
            return true;
        }

        public bool ValidateForAdd(
            string nameInput,
            IEnumerable<DishType> existingTypes,
            out string err)
        {
            err = string.Empty;
            if (!ValidName(nameInput, out err, existingTypes, checkExists: true)) return false;

            return true;
        }

        public bool ValidateForUpdate(
            DishType d,
            IEnumerable<DishType> existingTypes,
            out string err)
        {
            err = string.Empty;

            if (!CheckIfNull(d, out err)) return false;
            //if (!ExistsById(d.DishType_ID, existingTypes, out err)) return false;

            // Name format + uniqueness (exclude the current dish type ID)
            if (!ValidName(d.DishType_Name, out err, existingTypes, checkExists: true, excludeId: d.DishType_ID))
                return false;

            return true;
        }
    }
}
