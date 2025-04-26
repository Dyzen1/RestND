namespace RestND.MVVM.Model
{
    // Represents a product used in a specific dish and how much of it is used
    public class ProductUsageInDish
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
        private int _Amount_Usage;

        public int Amount_Usage
        {
            get { return _Amount_Usage; }
            set { _Amount_Usage = value; }
        }
        #endregion
    }
}