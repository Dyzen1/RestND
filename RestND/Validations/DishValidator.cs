using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.Dishes;
using RestND.MVVM.ViewModel.Dishes; // For SelectableProduct, SelectableItem<T>
using System;
using System.Collections.Generic;
using System.Linq;

namespace RestND.Validations
{
    public class DishValidator : GeneralValidations
    {
        private readonly DishServices _dishService = new();

        // ----------------- Null / basic checks -----------------

        public bool CheckIfNull(Dish dish, out string err)
        {
            err = string.Empty;
            if (dish == null)
            {
                err = "You must choose/create a dish.";
                return false;
            }
            return true;
        }

        public bool ExistsById(int dishId, IEnumerable<Dish> existingDishes, out string err)
        {
            err = string.Empty;
            if (existingDishes == null || !existingDishes.Any(d => d.Dish_ID == dishId && d.Is_Active))
            {
                err = "Dish does not exist.";
                return false;
            }
            return true;
        }

        // ----------------- Field-level checks (Employee-style) -----------------

        public bool ValidName(
            string name,
            out string err,
            IEnumerable<Dish>? existing = null,
            bool checkExists = true,
            int? excludeId = null)
        {
            err = string.Empty;

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
                    d.Dish_Name.Equals(name, StringComparison.OrdinalIgnoreCase) &&
                    (!excludeId.HasValue || d.Dish_ID != excludeId.Value));

                if (exists)
                {
                    err = "Dish with this name already exists!";
                    return false;
                }
            }

            return true;
        }

        public bool ValidPrice(string priceInput, out int parsed, out string err)
        {
            err = string.Empty;
            parsed = 0;

            if (!int.TryParse(priceInput, out parsed))
            {
                err = "Price must be a valid number.";
                return false;
            }

            if (!CheckPosNum(parsed, out var posErr))
            {
                err = posErr;
                return false;
            }

            return true;
        }

        public bool ValidDishType(DishType type, out string err)
        {
            err = string.Empty;
            if (type == null)
            {
                err = "Please select a dish type.";
                return false;
            }
            return true;
        }

        public bool ValidAllergens(IEnumerable<SelectableItem<string>> allergenOptions, out string err)
        {
            err = string.Empty;
            if (allergenOptions == null || !allergenOptions.Any(a => a.IsSelected))
            {
                err = "Please select at least one allergen note.";
                return false;
            }
            return true;
        }

        public bool ValidProducts(IEnumerable<SelectableProduct> productSelections, out string err)
        {
            err = string.Empty;
            if (productSelections == null || !productSelections.Any(p => p.IsSelected && p.AmountUsage > 0))
            {
                err = "Please select at least one product with a positive amount.";
                return false;
            }
            return true;
        }

        public bool ValidateForAdd(
            string nameInput,
            string priceInput,
            DishType dishTypeInput,
            IEnumerable<SelectableItem<string>> allergenOptions,
            IEnumerable<SelectableProduct> productSelections,
            IEnumerable<Dish> existingDishes,
            out int parsedPrice,
            out string err)
        {
            parsedPrice = 0;
            err = string.Empty;

            if (!ValidName(nameInput, out err, existingDishes, checkExists: true)) return false;
            if (!ValidPrice(priceInput, out parsedPrice, out err)) return false;
            if (!ValidDishType(dishTypeInput, out err)) return false;
            if (!ValidAllergens(allergenOptions, out err)) return false;
            if (!ValidProducts(productSelections, out err)) return false;

            return true;
        }

        /// <summary>
        /// Full validation for updating an existing dish (price already numeric on the model).
        /// Ensures the dish exists and the new name is not taken by another dish.
        /// </summary>
        public bool ValidateForUpdate(
            Dish dish,
            IEnumerable<Dish> existingDishes,
            IEnumerable<SelectableItem<string>> allergenOptions,
            IEnumerable<SelectableProduct> productSelections,
            out string err)
        {
            err = string.Empty;

            if (!CheckIfNull(dish, out err)) return false;
            if (!ExistsById(dish.Dish_ID, existingDishes, out err)) return false;

            // Name format + uniqueness (exclude the current dish ID)
            if (!ValidName(dish.Dish_Name, out err, existingDishes, checkExists: true, excludeId: dish.Dish_ID))
                return false;

            if (!ValidDishType(dish.Dish_Type, out err)) return false;
            if (!ValidAllergens(allergenOptions, out err)) return false;
            if (!ValidProducts(productSelections, out err)) return false;

            // Price already on the model; ensure positive
            if (!CheckPosNum(dish.Dish_Price, out err)) return false;

            return true;
        }
    }
}
