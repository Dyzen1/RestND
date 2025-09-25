using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Dishes;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace RestND.Validations
{
    public class SoftDrinkValidator : GeneralValidations
    {
        private readonly SoftDrinkServices _softDrinkService = new();

        public bool CheckIfNull(SoftDrink drink, out string err)
        {
            err = string.Empty;
            if (drink == null)
            {
                err = "You must choose/create a soft drink.";
                return false;
            }
            return true;
        }

        public bool ValidId(int id, out string err, bool checkExists = true)
        {
            err = string.Empty;

            if (id <= 0)
            {
                err = "Drink ID must be a valid number greater than 0!";
                return false;
            }

            if (checkExists)
            {
                var exists = _softDrinkService.GetAll().Any(d => d.Drink_ID == id);
                if (exists)
                {
                    err = "A soft drink with this ID already exists!";
                    return false;
                }
            }

            return true;
        }

        public bool ValidDishType(string type, out string err)
        {
            err = string.Empty;

            if (type == null)
            {
                err = "DishType is required.";
                return false;
            }

            // Must always be SoftDrinks (case-insensitive)
            if (!string.Equals(type, "SoftDrinks", StringComparison.OrdinalIgnoreCase))
            {
                err = "DishType must be set to 'SoftDrinks'.";
                return false;
            }

            return true;
        }

        public bool ValidName(string? name, out string err, bool checkExists = true, int? excludeId = null)
        {
            err = string.Empty;

            if (string.IsNullOrWhiteSpace(name))
            {
                err = "Drink name is required.";
                return false;
            }

            // Allow all letters (Unicode), digits, spaces, basic punctuation often used in product names
            if (!Regex.IsMatch(name, @"^[\p{L}\p{N}\s\-\.'()]+$"))
            {
                err = "Drink name can contain letters, numbers, spaces, -, ', (, ) and . only.";
                return false;
            }

            if (checkExists)
            {
                var exists = _softDrinkService.GetAll()
                    .Any(d => d.Drink_Name != null
                           && d.Drink_Name.Equals(name, StringComparison.OrdinalIgnoreCase)
                           && (!excludeId.HasValue || d.Drink_ID != excludeId.Value));

                if (exists)
                {
                    err = "A soft drink with this name already exists!";
                    return false;
                }
            }

            return true;
        }

        public bool ValidPrice(double price, out string err)
        {
            err = string.Empty;

            if (price < 0)
            {
                err = "Price cannot be negative.";
                return false;
            }

            return true;
        }

        public bool ValidQuantity(int qty, out string err)
        {
            err = string.Empty;

            if (qty < 0)
            {
                err = "Quantity cannot be negative.";
                return false;
            }

            return true;
        }

        // ---- Full-form helpers ----

        public bool ValidateForAdd(SoftDrink d, out string err)
        {
            if (!CheckIfNull(d, out err)) return false;
            if (!ValidDishType(d.DishType, out err)) return false;
            if (!ValidName(d.Drink_Name, out err, checkExists: true)) return false;
            if (!ValidPrice(d.Drink_Price, out err)) return false;
            if (!ValidQuantity(d.Quantity, out err)) return false;

            // ID for add: if you set it manually, ensure uniqueness too
            if (d.Drink_ID > 0 && !ValidId(d.Drink_ID, out err, checkExists: true)) return false;

            err = string.Empty;
            return true;
        }

        public bool ValidateForUpdate(SoftDrink d, out string err)
        {
            if (!CheckIfNull(d, out err)) return false;
            if (!ValidDishType(d.DishType, out err)) return false;
            if (!ValidName(d.Drink_Name, out err, checkExists: true, excludeId: d.Drink_ID)) return false;
            if (!ValidPrice(d.Drink_Price, out err)) return false;
            if (!ValidQuantity(d.Quantity, out err)) return false;

            // ID exists but shouldn’t be checked for “already exists” against itself
            if (!ValidId(d.Drink_ID, out err, checkExists: false)) return false;

            err = string.Empty;
            return true;
        }
    }
}
