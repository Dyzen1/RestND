using CommunityToolkit.Mvvm.ComponentModel;
using RestND.MVVM.Model.Dishes;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using System;

namespace RestND.MVVM.Model
{
    public class Dish : ObservableObject
    {
        #region Dish_ID
        private int _Dish_ID;
        public int Dish_ID
        {
            get => _Dish_ID;
            set => SetProperty(ref _Dish_ID, value);
        }
        #endregion

        #region Dish_Name
        private string? _Dish_Name;
        public string? Dish_Name
        {
            get => _Dish_Name;
            set => SetProperty(ref _Dish_Name, value);
        }
        #endregion

        #region Dish_Price
        private int _Dish_Price;
        public int Dish_Price
        {
            get => _Dish_Price;
            set => SetProperty(ref _Dish_Price, value);
        }
        #endregion

        #region Is_Active - a property for knowing wheather the dish has been deleted or not
        private bool _Is_Active;
        public bool Is_Active
        {
            get => _Is_Active; 
            set => SetProperty(ref _Is_Active, value);
        }
        #endregion

        #region Allergen_Notes
        private string _Allergen_Notes;

        public string Allergen_Notes
        {
            get => _Allergen_Notes; 
            set => SetProperty(ref _Allergen_Notes, value);
        }
        #endregion

        #region Availability_Status
        private bool _Availability_Status;

        public bool Availability_Status
        {
            get => _Availability_Status;
            set => SetProperty(ref _Availability_Status, value);
        }
        #endregion

        #region ProductUsage
        private List<ProductInDish> _ProductUsage = new();

        public List<ProductInDish> ProductUsage
        {
            get => _ProductUsage;
            set => SetProperty(ref _ProductUsage, value);
        }
        #endregion

        #region Dish_Type
        private DishType? _Dish_Type;

        public DishType? Dish_Type
        {
            get => _Dish_Type;
            set => SetProperty(ref _Dish_Type, value);
        }
        #endregion

        #region Constructor with allergen notes
        public Dish(string? dishName, int dishPrice, string allergenNotes, List<ProductInDish> productUsage, DishType? type)
        {
            this.Dish_Name = dishName;
            this.Dish_Price = dishPrice;
            this.Allergen_Notes = allergenNotes;
            this.Availability_Status = true;
            this.ProductUsage = productUsage;
            this.Dish_Type = type;
            this.Is_Active = true;
        }

        #endregion

        #region Constructor without allergen notes
        public Dish(string? dishName, int dishPrice, List<ProductInDish> productUsage, DishType? type)
        {
            this.Dish_Name = dishName;
            this.Dish_Price = dishPrice;
            this.Availability_Status = true;
            this.ProductUsage = productUsage;
            this.Dish_Type = type;
            this.Is_Active = true;
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
