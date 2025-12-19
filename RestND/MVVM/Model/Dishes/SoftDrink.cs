using System;

namespace RestND.MVVM.Model.Dishes
{
    public class SoftDrink
    {
        #region Drink ID
        private int _Drink_ID;
        public int Drink_ID
        {
            get => _Drink_ID;
            set => _Drink_ID = value;
        }
        #endregion

        #region Drink name
        private string? _Drink_Name;
        public string? Drink_Name
        {
            get => _Drink_Name;
            set => _Drink_Name = value;
        }
        #endregion

        #region DishType
        public string DishType { get; } = "SoftDrinks";
        #endregion

        #region Price
        private double _Drink_Price;
        public double Drink_Price
        {
            get => _Drink_Price;
            set => _Drink_Price = value;
        }
        #endregion

        #region Quantity
        private int _Quantity;
        public int Quantity
        {
            get => _Quantity;
            set => _Quantity = value;
        }
        #endregion

        #region Is_Active 
        private bool _Is_Active;
        public bool Is_Active
        {
            get => _Is_Active;
            set => _Is_Active = value;
        }
        #endregion

            #region Ctors
            public SoftDrink() { }

            public SoftDrink(int drink_ID, string? drink_Name, double drink_Price, int quantity, bool is_Active)
            {
                Drink_ID = drink_ID;
                Drink_Name = drink_Name;
                Drink_Price = drink_Price;
                Quantity = quantity;
                Is_Active = is_Active;
            }
            #endregion
    }
}
