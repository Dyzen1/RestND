namespace RestND.MVVM.Model
{
    // Represents a product used in a specific dish and how much of it is used
    public class ProductInDish
    {
        #region Product_ID
        // The ID of the product (linked from the 'inventory' table)
        private string _Product_ID;

        public string Product_ID
        {
            get { return _Product_ID; }
            set { _Product_ID = value; }
        }
        #endregion

        #region Amount_Usage
        // How much of the product is used in the dish (e.g., 150 grams, 200 ml)
        private double _Amount_Usage;

        public double Amount_Usage
        {
            get { return _Amount_Usage; }
            set { _Amount_Usage = value; }
        }
        #endregion

        #region Dish_ID
        // The ID of the dish (linked from the 'dishes' table)
        private int _Dish_ID;

        public int Dish_ID
        {
            get { return _Dish_ID; }
            set { _Dish_ID = value; }
        }
        #endregion

        #region Product_Name
        private string _Product_Name;

        public string Product_Name
        {
            get { return _Product_Name; }
            set { _Product_Name = value; }
        }
        #endregion

        #region Is_Active
        private bool _Is_Active = true; 
        public bool Is_Active
        {
            get { return _Is_Active; }
            set { _Is_Active = value; }
        }
        #endregion

        #region Constructor
        public ProductInDish(string Product_ID, int Dish_ID, string Product_Name, double Amount_Usage)
        {
            this.Product_ID = Product_ID; 
            this.Dish_ID = Dish_ID;
            this.Amount_Usage = Amount_Usage;
            this.Product_Name = Product_Name;
            this.Is_Active = true;
        }
        #endregion

        #region Default Constructor
        public ProductInDish()
        {
            
        }
        #endregion
    }
}