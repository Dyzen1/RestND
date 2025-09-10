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
        #endregion


    }
}
