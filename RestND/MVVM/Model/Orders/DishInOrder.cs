using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model.Orders
{
    public partial class DishInOrder : ObservableObject
    {
        public Dish dish { get; }

        [ObservableProperty] private int quantity;
        [ObservableProperty] private int totalDishPrice;

        public DishInOrder(Dish dish)
        {
            this.dish = dish;
            this.Quantity = 1;
            this.TotalDishPrice = dish.Dish_Price;
        }
    }
}
