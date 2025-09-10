
//using RestND.MVVM.Model.Orders;
//using RestND.Data;

//namespace RestND.MVVM.ViewModel.Orders
//{
//    public partial class BillViewModel
//    {
//        private readonly BillServices _billServices = new BillServices();

//        #region Generate Bill From Order
//        public Bill GenerateBillForOrder(Order order, Discount? discount = null)
//        {
//            double totalPrice = 0;
//            var DishesInOrder = order.DishInOrder;
//            Bill bill= new Bill();

//            for (int i = 0; i < order.DishInOrder.Count; i++)
//            {
//                totalPrice += order.DishInOrder[i].Dish.Dish_Price * order.DishInOrder[i].Quantity;

//            }
//            totalPrice = totalPrice * _billServices.Vat;

//            if (discount != null)
//            {
//                totalPrice -= totalPrice * (discount.Discount_Percentage / 100.0);

//                bill = new Bill(order, discount, totalPrice);
               
//            }
//            else
//            {
//                 bill = new Bill(order, totalPrice);
//            }

            
//            _billServices.Add(bill);
//            return bill;
//        }
//        #endregion

//    }
//}
