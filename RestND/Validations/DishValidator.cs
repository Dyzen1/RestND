using ControlzEx.Standard;
using RestND.Data;
using RestND.MVVM.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.Validations
{
    public class DishValidator : GeneralValidations
    {
        Dish dish;

        public DishValidator(Dish dish)
        {
            this.dish = dish;
        }

        public bool checkIfNameExists(out string errorMessage)
        {
            errorMessage = string.Empty;
            string name = this.dish.Dish_Name;
            DishServices dishServices = new DishServices();
            List<Dish> dishes = dishServices.GetAll();
            if(!dishes.Any(dish => dish.Dish_Name==name))
            {
                errorMessage = "Dish already exists!";
                return false;
            }
            return true;
        }

        public bool dishPrice_Validation(out string errorMessage)
        {
            errorMessage = string.Empty;
            if(string.IsNullOrEmpty(this.dish.Dish_Price.ToString()))
            {
                errorMessage = "Insert price!";
                return false;
            }
            if (this.dish.Dish_Price <= 0)
            {
                errorMessage = "Price can't be lower then zero.";
                return false;
            }
            return true;
        }

        public bool doesHaveProducts(out string errorMessage)
        {
            errorMessage = string.Empty;
            if (this.dish.ProductUsage.Count == 0)
            {
                errorMessage = "Dish has no products!";
                return false;
            }
            return true;
        }




    }
}
