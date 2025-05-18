namespace RestND.MVVM.Model
{
    // Represents a product used in a specific dish and how much of it is used
    public class ProductInDish
    {
        #region Product_ID
        // The ID of the product (linked from the 'product' table)
        private int _Product_ID;

        public int Product_ID
        {
            get { return _Product_ID; }
            set { _Product_ID = value; }
        }
        #endregion

        #region Product_Name
        // The name of the product (e.g., "Tomato", "Cheese")
        private string _Product_Name;

        public string Product_Name
        {
            get { return _Product_Name; }
            set { _Product_Name = value; }
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
        private string _Dish_ID;

        public string Dish_ID
        {
            get { return _Dish_ID; }
            set { _Dish_ID = value; }
        }
        #endregion

        #region Constructor

        public ProductInDish(string Product_Name, double amount_Usage)
        {
            this.Product_Name = Product_Name;
            this.Amount_Usage = Amount_Usage;
        }

        #endregion

        #region Default Constructor

        public ProductInDish()
        {
            
        }
        #endregion
    }
}