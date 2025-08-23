using System;
using System.Collections.Generic;
using System.Linq;
using RestND.MVVM.Model;
using RestND.Data;

namespace RestND.Validations
{
    public class DishValidator : GeneralValidations
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

        public bool CheckIfNull(Dish dish, out string err)
        {
            err = string.Empty;
            if (dish == null)
            {
                err = "You must choose a dish to update";
                return false;
            }
            return true;
        }
    }
}
