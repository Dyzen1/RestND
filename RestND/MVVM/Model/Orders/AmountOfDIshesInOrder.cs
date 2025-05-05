using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model.Orders
{
    public class AmountOfDIshesInOrder
    {
        #region Dish
        private Dish _Dish;

        public Dish Dish { 
            get{return _Dish; }

            set {_Dish = value; } 
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


        #region Constructor
        public AmountOfDIshesInOrder(Dish dish,int amount)
        {
            _Amount = amount;
            _Dish = dish;
        }
        #endregion

    }
}
