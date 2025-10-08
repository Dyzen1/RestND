using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model.Orders
{
    public partial class OrderLine : ObservableObject
    {
        public Dish dish { get; }

        [ObservableProperty] private int quantity;
        [ObservableProperty] private int lineTotal;

        public OrderLine(Dish dish, int qty = 1)
        {
            this.dish = dish;
            this.Quantity = qty;
            this.lineTotal = dish.Dish_Price;
        }
    }
}
