using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model
{
    public class DishType
    {
        #region Dish Type ID

        private int _DishType_ID;

        public int DishType_ID
        {
            get { return _DishType_ID; }
            set { _DishType_ID = value; }
        }

        #endregion

        #region Dish Type Name

        private string? _DishType_Name;

        public string? DishType_Name
        {
            get { return _DishType_Name; }
            set { _DishType_Name = value; }
        }

        #endregion

        #region Is_Active - a property for knowing wheather the dishType has been deleted or not
        private bool _Is_Active;
        public bool Is_Active
        {
            get { return _Is_Active; }
            set { _Is_Active = value; }
        }
        #endregion

        #region Constructor
        public DishType( string? dishType_Name)
        {
            this.DishType_Name = dishType_Name;
            this.Is_Active = true; 
        }

        #endregion

        #region Default Constructor

        public DishType()
        {

        }

        #endregion

    }
}
