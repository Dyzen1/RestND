

namespace RestND.MVVM.Model.Orders
{
    public class DishInOrder
    {
        #region Dish
        private Dish _Dish;
        public Dish Dish
        {
            get { return _Dish; }
            set
            {
                if (_Dish != null)
                    _Dish = value;
            }
        }
        #endregion

        #region Quantity Of Dish
        private int _Quantity;
        public int Quantity
        {
            get{return _Quantity; }
            set 
            { 
                if(_Quantity> 0)
                _Quantity = value;
                else
                {
                    _Quantity = 0;
                }
            }
        }
        #endregion

    }
}
