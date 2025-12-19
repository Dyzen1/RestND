using System;
using System.Collections.Generic;
using System.Linq;
using RestND.Data;
using RestND.MVVM.Model.Orders;

namespace RestND.MVVM.ViewModel.Orders
{
    public partial class BillViewModel
    {
        private readonly BillServices _billServices = new();
        private readonly DishServices _dishServices = new();

        #region Generate Bill From Order
        public Bill GenerateBillForOrder(Order order, Discount? discount = null)
        {
            if (order == null) throw new ArgumentNullException(nameof(order));

            IEnumerable<DishInOrder> items = order.DishInOrder ?? Enumerable.Empty<DishInOrder>();

            double subtotal = 0.0;
            foreach (var line in items)
            {
                if (line == null) continue;
                var unitPrice = ResolveUnitPrice(line); // no line.Dish usage
                var qty = Math.Max(1, line.Quantity);
                subtotal += unitPrice * qty;
            }

            // VAT: support either 0.17 (rate) or 1.17 (multiplier)
            double vat = _billServices.Vat;
            double totalWithVat = (vat >= 1.0) ? subtotal * vat : subtotal * (1.0 + vat);

            if (discount is { Discount_Percentage: > 0 })
            {
                totalWithVat -= totalWithVat * (discount.Discount_Percentage / 100.0);
            }

            var bill = new Bill(order, totalWithVat);
            if (discount != null) bill.Discount = discount;

            _billServices.Add(bill);
            return bill;
        }
        #endregion

        #region Helpers
        private double ResolveUnitPrice(DishInOrder line)
        {
            // If your line class stores its own price field, prefer it:
            // return line.UnitPrice;
            // return line.Dish_Price;

            // Otherwise, resolve by Dish_ID via DishServices
            var dish = _dishServices.GetById(line.dish.Dish_ID);
            return dish?.Dish_Price ?? 0.0;
        }
        #endregion
    }
}
