using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.Model
{
    public class Dish
    {
        #region Dish ID

        private int _Dish_ID;

        public int Dish_ID
        {
            // need to add validation logic here 
            get { return _Dish_ID; }
            set { _Dish_ID = value; }
        }


        #endregion

        #region Dish name

        private string _Dish_Name;

        public string Dish_Name
        {
            get { return _Dish_Name; }
            set { _Dish_Name = value; }
        }

        #endregion

        #region _Dish_Price

        private int _Dish_Price;

        public int Dish_Price
        {
            get { return _Dish_Price; }
            set { _Dish_Price = value; }
        }

        #endregion

        #region Allergen_Notes

        private string _Allergen_Notes;

        public string Allergen_Notes
        {
            get { return _Allergen_Notes; }
            set { _Allergen_Notes = value; }
        }

        #endregion

        #region _Availability_Status
        private bool _Availability_Status;

        public bool Availability_Status
        {
            get { return _Availability_Status; }
            set { _Availability_Status = value; }
        }

        #endregion

        #region Amount Products Usage

        private List<Product> _ProductUsage;

        public List<Product> ProductUsage
        {
            get { return _ProductUsage; }
            set { _ProductUsage = value; }
        }

        #endregion

        #region constructor




        public Dish(int dish_Id, string dishName, string allergenNotes,bool availabilityStatus , List<Product> productUsage) 
        {
            Dish_ID = dish_Id;
            Dish_Name = dishName;
            Allergen_Notes = allergenNotes;
            Availability_Status = availabilityStatus;
            ProductUsage = productUsage;
        }
        #endregion

        #region Default Constructor

        public Dish()
        {
            
        }

        #endregion

        #region Equals Method override
        public override bool Equals(object obj)
        {
            if (obj is Dish other)
                return _Dish_ID == other._Dish_ID;

            return false;

        }
        #endregion

    }
}
