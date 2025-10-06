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
        public Dish Dish { get; }

        [ObservableProperty] private int quantity;

        public int LineTotal => (Dish?.Dish_Price ?? 0) * Quantity;

        public OrderLine(Dish dish, int qty = 1)
        {
            Dish = dish;
            Quantity = qty;
        }
    }
}
