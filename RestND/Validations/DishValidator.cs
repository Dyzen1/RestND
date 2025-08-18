using System;
using System.Collections.Generic;
using System.Linq;
using RestND.MVVM.Model;
using RestND.Data;

namespace RestND.Validations
{
    public class DishValidator
    {

        private readonly DishServices _dishService = new();

        public bool CheckIfExists(string dishName, out string err)
        {
            err = string.Empty;
            List<Dish> dishes = _dishService.GetAll();
            var doesExist = dishes.FirstOrDefault(d => d.Dish_Name == dishName && d.Is_Active);
            if (doesExist != null)
            {
                err = "Dish already exists!";
                return false;
            }
            return true;
        }

        public bool checkPricePos(int price, out string err)
        {
            err = string.Empty;
            if (price <= 0)
            {
                err = "Price must be a positive number!";
                return false;
            }
            return true;
        }

        public bool CheckDishType(string dishType, out string err)
        {
            err = string.Empty;
            if (dishType == null)
            {
                err = "You must choose a dish type";
                return false;
            }
            return true;
        }


        // Method to validate dish fields
        //public static Dictionary<string, List<string>> ValidateFields(Dish dish, List<Dish> existingDishes)
        //{
        //    var errors = new Dictionary<string, List<string>>();

        //    // Dish Name Validation
        //    if (string.IsNullOrWhiteSpace(dish.Dish_Name))
        //    {
        //        AddError(errors, nameof(dish.Dish_Name), "Dish name is required!");
        //    }
        //    else if (dish.Dish_Name.Length < 3)
        //    {
        //        AddError(errors, nameof(dish.Dish_Name), "Dish name must be at least 3 characters long.");
        //    }
        //    else if (existingDishes.Any(d => d.Dish_Name.Equals(dish.Dish_Name, StringComparison.OrdinalIgnoreCase)))
        //    {
        //        AddError(errors, nameof(dish.Dish_Name), "Dish already exists!");
        //    }

        //    // Dish Price Validation
        //    if (dish.Dish_Price <= 0)
        //    {
        //        AddError(errors, nameof(dish.Dish_Price), "Price can't be lower than zero.");
        //    }

        //    // Dish Product Usage Validation
        //    if (dish.ProductUsage == null || dish.ProductUsage.Count == 0)
        //    {
        //        AddError(errors, nameof(dish.ProductUsage), "Dish has no products!");
        //    }

        //    return errors;
        //}

        //// Helper method to add errors to dictionary
        //private static void AddError(Dictionary<string, List<string>> dict, string key, string message)
        //{
        //    if (!dict.ContainsKey(key))
        //        dict[key] = new List<string>();
        //    dict[key].Add(message);
        //}
    }
}
