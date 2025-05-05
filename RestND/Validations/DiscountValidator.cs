using RestND.MVVM.Model.Orders;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestND.Validations
{
    public static class DiscountValidator
    {
        public static Dictionary<string, List<string>> ValidateFields(Discount discount, List<Discount> existingDiscounts, bool checkNameExists = true)
        {
            var errors = new Dictionary<string, List<string>>();

            // Name validation
            if (string.IsNullOrWhiteSpace(discount.Discount_Name))
            {
                AddError(errors, nameof(discount.Discount_Name), "Insert discount name!");
            }
            else if (discount.Discount_Name.Length < 3)
            {
                AddError(errors, nameof(discount.Discount_Name), "Discount name must be at least 3 characters long.");
            }

            // Percentage validation
            if (discount.Discount_Percentage <= 0)
            {
                AddError(errors, nameof(discount.Discount_Percentage), "Discount percentage can't be lower than zero.");
            }
            else if (discount.Discount_Percentage > 100)
            {
                AddError(errors, nameof(discount.Discount_Percentage), "Discount percentage can't be higher than 100.");
            }

            // Check for duplicate name (if needed)
            if (checkNameExists && existingDiscounts.Any(d =>
                d.Discount_Name.Equals(discount.Discount_Name, StringComparison.OrdinalIgnoreCase) &&
                d.Discount_ID != discount.Discount_ID))
            {
                AddError(errors, nameof(discount.Discount_Name), "Discount name already exists!");
            }

            return errors;
        }

        private static void AddError(Dictionary<string, List<string>> dict, string key, string message)
        {
            if (!dict.ContainsKey(key))
                dict[key] = new List<string>();
            dict[key].Add(message);
        }
    }
}