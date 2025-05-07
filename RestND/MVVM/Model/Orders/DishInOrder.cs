using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model.Orders
{
    public class DishInOrder
    {
        #region Dish_ID
        private string _Dish_ID;

        public string Dish_ID
        { 
           get { return _Dish_ID; }
           set { _Dish_ID = value; }
        }
        #endregion

        #region Order_ID
        private string _Order_ID;
        public string Order_ID
        {
            get { return _Order_ID; }
            set { _Order_ID = value; }
        }
        #endregion

        #region Dish_Name
        private string _Dish_Name;
        public string Dish_Name
        {
            get { return _Dish_Name; }
            set { _Dish_Name = value; }
        }
        #endregion

        #region Amount Of Dish
        private int _Amount;
        public int Amount
        {
            get{return _Amount;}
            set {_Amount = value; }
        }
        #endregion

    }
}
