using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model
{
    public class DishType
    {
        #region DIsh ID

        private int _DishType_ID;

        public int DishType_ID
        {
            get { return _DishType_ID; }
            set { _DishType_ID = value; }
        }

        #endregion

        #region Dish Type Name

        private string _DishType_Name;

        public string DishType_Name
        {
            get { return DishType_Name; }
            set { _DishType_Name = value; }
        }

        #endregion

        #region Constructor

        public DishType( string dishType_Name)
        {
            
            _DishType_Name = dishType_Name;
        }

        #endregion

        #region Default Constructor

        public DishType()
        {

        }

        #endregion

    }
}
