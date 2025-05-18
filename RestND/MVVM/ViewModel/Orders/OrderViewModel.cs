using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using RestND.MVVM.Model.Orders;
using RestND.Data;
using RestND.MVVM.Model;
using System.Collections.Generic;

namespace RestND.MVVM.ViewModel.Orders
{
    public partial class OrderViewModel : ObservableObject
    {
        #region Services
        private readonly OrderServices _orderService;
        private readonly DishInOrderServices _dishInOrderServices;
        private readonly DishServices _dishServices;
        private readonly ProductService _inventoryServices;
        #endregion

        #region Fields
        private Dictionary<string, double> _productInventory = new();

        [ObservableProperty]
        public ObservableCollection<Order> orders = new ObservableCollection<Order>();

        [ObservableProperty]
        public ObservableCollection<DishInOrder> dishesInCurrentOrder = new ObservableCollection<DishInOrder>(); 

        [ObservableProperty]
        public Dish newDish; 

        [ObservableProperty]
        public Order selectedOrder;

        [ObservableProperty]
        public Order newOrder;

        #endregion

        #region On change (for buttons)
        partial void OnSelectedOrderChanged(Order value)
        {
            //DeleteOrderCommand.NotifyCanExecuteChanged();
            //UpdateOrderCommand.NotifyCanExecuteChanged();
            
        }

        #endregion

        #region Constructor
        public OrderViewModel()
        {
            _dishInOrderServices = new DishInOrderServices();
            _orderService = new OrderServices();
            _dishServices = new DishServices();
            _inventoryServices = new ProductService();
            LoadProductInventory();
        }

        #endregion

        #region Load Orders

        [RelayCommand] //for real-time.
        private void LoadOrders()
        {
            Orders.Clear();
            var dbOrders = _orderService.GetAll(); 

            foreach (var order in dbOrders)
            {
                Orders.Add(order);
            }
        }

        #endregion

        #region Begin Order

        [RelayCommand(CanExecute = nameof(CanStartNewOrder))]
        private void BeginOrder()
        {
            UpdateDishesAvailibility();
            bool success = _orderService.Add(NewOrder);

            if (success)
            {
                DishesInCurrentOrder.Clear();
                SelectedOrder = NewOrder;
                NewOrder = new Order();
            }
        }

        #endregion

        #region Delete Order

        [RelayCommand(CanExecute = nameof(CanModifyOrder))]
        private void DeleteProduct()
        {
            if (SelectedOrder != null)
            {
                bool success = _orderService.Delete(SelectedOrder.Order_ID);

                if (success)
                {
                    Orders.Remove(SelectedOrder);
                }
            }
        }

        #endregion

        #region Update Order

        [RelayCommand(CanExecute = nameof(CanModifyOrder))]
        private void UpdateOrder()
        {
            bool success = _orderService.Update(SelectedOrder);
            if (success)
            {
                int index = Orders.IndexOf(SelectedOrder);
                if (index >= 0)
                    Orders[index] = SelectedOrder;
            }
        }

        #endregion

        #region Add Dish to Order

        [RelayCommand(CanExecute = nameof(CanAddDishToOrder))]
        private void AddDishToOrder(int quantity)
        {
            DishInOrder dishInOrder = new DishInOrder
            {
                Dish = NewDish,
                Quantity = quantity
            };
            bool success = _dishInOrderServices.AddDishToOrder(SelectedOrder.Order_ID, dishInOrder);

            if (success)
            {
                DishesInCurrentOrder.Add(dishInOrder);
                updateBillSum();

                foreach (var product in dishInOrder.Dish.ProductUsage)
                {
                    string productId = product.Product_ID;
                    double totalToSubtract = product.Amount_Usage * dishInOrder.Quantity;

                    if (_productInventory.ContainsKey(productId))
                    {
                        _productInventory[productId] -= totalToSubtract;
                        _inventoryServices.UpdateProductQuantity(productId, _productInventory[productId]);
                    }
                }


            }
        }

        #endregion
   
        #region Remove Dish from Current order

        [RelayCommand]
        public void RemoveDishFromOrder(DishInOrder dishInOrder)
        {
            if (SelectedOrder != null && dishInOrder != null)
            {
                bool success = _dishInOrderServices.DeleteDishFromOrder(dishInOrder.Dish.Dish_ID, SelectedOrder.Order_ID);
                if (success)
                {
                    DishesInCurrentOrder.Remove(dishInOrder);
                    updateBillSum();

                    foreach (var product in dishInOrder.Dish.ProductUsage)
                    {
                        string productId = product.Product_ID;
                        double totalToRestore = product.Amount_Usage * dishInOrder.Quantity;

                        if (_productInventory.ContainsKey(productId))
                        {
                            _productInventory[productId] += totalToRestore;
                            _inventoryServices.UpdateProductQuantity(productId, _productInventory[productId]);
                        }
                    }


                }
            }
        }

        #endregion

        #region UpdateBill
        private void updateBillSum()
        {
            double sum = 0;
            foreach (var dish in DishesInCurrentOrder)
            {
                sum += dish.Dish.Dish_Price * dish.Quantity;
            }
            SelectedOrder.Bill.Price = sum;
        }

        #endregion

        #region Dishes checks!
        private void UpdateDishesAvailibility()
        {
            _dishServices.UpdateDishesAvailibility(); //a different method.
        }

        #endregion

        #region A method to load the product inventory into a dictionary
        private void LoadProductInventory()
        {
            _productInventory = _inventoryServices.GetProductDictionary();
        }
        #endregion

        #region CanExecute 
        private bool CanModifyOrder()
        {
            return SelectedOrder != null;
        }
        private bool CanStartNewOrder()
        {
            return NewOrder != null && NewOrder.Table != null && NewOrder.assignedEmployee != null;
        }
        private bool CanAddDishToOrder()
        {
            return SelectedOrder != null && SelectedOrder.DishInOrder != null;
        }

        #endregion

    }
}
