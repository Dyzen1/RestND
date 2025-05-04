using RestND.MVVM.Model.Dishes;
using System.Collections.Generic;

namespace RestND.MVVM.Model
{
    public class Dish
    {
        #region Dish_ID
        private int _Dish_ID;

        public int Dish_ID
        {
            get { return _Dish_ID; }
            set { _Dish_ID = value; }
        }
        #endregion

        #region Dish_Name
        private string? _Dish_Name;

        public string? Dish_Name
        {
            get { return _Dish_Name; }
            set { _Dish_Name = value; }
        }
        #endregion

        #region Dish_Price
        private double _Dish_Price;

        public double Dish_Price
        {
            get { return _Dish_Price; }
            set { _Dish_Price = value; }
        }
        #endregion

        #region Allergen_Notes
        private AllergenNotes _Allergen_Notes;

        public AllergenNotes Allergen_Notes
        {
            get { return _Allergen_Notes; }
            set { _Allergen_Notes = value; }
        }
        #endregion

        #region Availability_Status
        private bool _Availability_Status;

        public bool Availability_Status
        {
            get { return _Availability_Status; }
            set { _Availability_Status = value; }
        }
        #endregion

        #region ProductUsage
        private List<ProductUsageInDish> _ProductUsage = new();

        public List<ProductUsageInDish> ProductUsage
        {
            get { return _ProductUsage; }
            set { _ProductUsage = value; }
        }
        #endregion

        #region Dish_Type
        private DishType? _Dish_Type;

        public DishType? Dish_Type
        {
            get { return _Dish_Type; }
            set { _Dish_Type = value; }
        }
        #endregion

        #region Constructor with allergen notes

        public Dish(string? dishName, double dishPrice, AllergenNotes allergenNotes, List<ProductUsageInDish> productUsage, DishType? type)
        {
            Dish_Name = dishName;
            Dish_Price = dishPrice;
            Allergen_Notes = allergenNotes;
            Availability_Status = true;
            ProductUsage = productUsage;
            Dish_Type = type;
        }

        #endregion

        #region Constructor without allergen notes

        public Dish(string? dishName, double dishPrice, List<ProductUsageInDish> productUsage, DishType? type)
        {
            Dish_Name = dishName;
            Dish_Price = dishPrice;
            Availability_Status = true;
            ProductUsage = productUsage;
            Dish_Type = type;
        }

        #endregion

        #region Default Constructor
        public Dish()
        {
            // Default constructor
        }
        #endregion
    }
}
