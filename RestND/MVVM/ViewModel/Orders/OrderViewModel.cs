using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.Input;
using RestND.MVVM.Model.Orders;
using RestND.Data;
using System;
using RestND.MVVM.Model;

namespace RestND.MVVM.ViewModel.Orders
{
    public partial class OrderViewModel : ObservableObject
    {
        #region Services
        private readonly OrderServices _orderService;
        #endregion

        #region Fields

        [ObservableProperty]
        public ObservableCollection<Order> orders = new ObservableCollection<Order>();

        [ObservableProperty]
        public Order selectedOrder;

        [ObservableProperty]
        public Order newOrder = new Order();

        partial void OnSelectedOrderChanged(Order value)
        {
            //DeleteOrderCommand.NotifyCanExecuteChanged();
            //UpdateOrderCommand.NotifyCanExecuteChanged();
        }

        #endregion

        #region Constructor
        public OrderViewModel()
        {
            _orderService = new OrderServices();
            LoadOrders();
        }
        #endregion

        #region Load Orders
        [RelayCommand] //for real-time.
        private void LoadOrders()
        {
            orders.Clear();

            var dbOrders = _orderService.GetAll(); 

            foreach (var order in dbOrders)
            {
                Orders.Add(order);
            }
        }

        #endregion

        #region Begin Order

        [RelayCommand]
        private void BeginOrder()
        {
            selectedOrder.Table.Table_Status = false;
        }

        #endregion

        #region Save Order
        [RelayCommand]
        private void SaveOrder()
        {
            bool success = _orderService.Add(NewOrder); 

            if (success)
            {
                NewOrder.Table.Table_Status = true;
                NewOrder = new Order();
            }
        }

        #endregion

        #region Delete Order

        [RelayCommand(CanExecute = nameof(CanModifyOrder))]
        private void DeleteProduct()
        {
            if (selectedOrder != null)
            {
                bool success = _orderService.Delete(selectedOrder.Order_ID);

                if (success)
                {
                    orders.Remove(selectedOrder);
                }
            }
        }

        #endregion

        //empty, need to add logic.
        #region Update Order

        #endregion 

        #region CanExecute Helpers

        private bool CanModifyOrder()
        {
            return selectedOrder != null;
        }

        #endregion

    }
}
