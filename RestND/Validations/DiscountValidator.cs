using RestND.MVVM.Model;
using RestND.MVVM.Model.Orders;
using RestND.MVVM.ViewModel.Dishes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestND.Validations
{
    public class DiscountValidator : GeneralValidations
    {
        private readonly DiscountService _discountService = new();
        public bool CheckIfNull(Discount d, out string err)
        {
            err = string.Empty;
            if (d == null)
            {
                err = "You must choose/create a discount.";
                return false;
            }
            return true;
        }

        public bool ValidName(
            string name,
            out string err,
            IEnumerable<Discount>? existing = null,
            bool checkExists = true,
            int? excludeId = null)
        {
            err = string.Empty;
            existing = _discountService.GetAll();

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
                    d.Discount_Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                    (!excludeId.HasValue || d.Discount_ID != excludeId.Value));

                if (exists)
                {
                    err = "Discount with this name already exists!";
                    return false;
                }
            }

            return true;
        }

        public bool ValidPercentage(string percentage, out double parsed, out string err)
        {
            err = string.Empty;
            parsed = 0;

            if (!double.TryParse(percentage, out parsed))
            {
                err = "Discount amount must be a valid number.";
                return false;
            }

            if (!CheckPosNumDouble(parsed, out var posErr))
            {
                err = posErr;
                return false;
            }

            return true;
        }

        public bool ExistsById(int discId, IEnumerable<Discount> existingDiscs, out string err)
        {
            err = string.Empty;
            existingDiscs = _discountService.GetAll();
            if (existingDiscs == null || !existingDiscs.Any(d => d.Discount_ID == discId && d.Is_Active))
            {
                err = "Discount does not exist.";
                return false;
            }
            return true;
        }

        public bool ValidateForAdd(
            string nameInput,
            string percentageInput,
            IEnumerable<Discount> existingDiscs,
            out double parsedPercentage,
            out string err)
        {
            parsedPercentage = 0;
            err = string.Empty;

            if (!ValidName(nameInput, out err, existingDiscs, checkExists: true)) return false;
            if (!ValidPercentage(percentageInput, out parsedPercentage, out err)) return false;

            return true;
        }

        public bool ValidateForUpdate(
            Discount d,
            IEnumerable<Discount> existingDiscs,
            out string err)
        {
            err = string.Empty;

            if (!CheckIfNull(d, out err)) return false;
            //if (!ExistsById(d.Discount_ID, existingDiscs, out err)) return false;

            // Name format + uniqueness (exclude the current discount ID)
            if (!ValidName(d.Discount_Name, out err, existingDiscs, checkExists: true, excludeId: d.Discount_ID))
                return false;

            // Price already on the model; ensure positive
            if (!CheckPosNumDouble(d.Discount_Percentage, out err)) return false;

            return true;
        }
    }
}