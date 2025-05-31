using CommunityToolkit.Mvvm.ComponentModel;


namespace RestND.MVVM.Model
{
    public class Inventory : ObservableObject
    {
        #region Product ID

        private string _Product_ID;

        public string Product_ID
        {
            get => _Product_ID;
            set => SetProperty(ref _Product_ID, value);
        }


        #endregion

        #region Product Name

        private string _Product_Name;

        public string Product_Name
        {
            get  => _Product_Name;
            set => SetProperty( ref _Product_Name , value); 
        }
        #endregion

        #region Tolerance
        private double _Tolerance;

        public double Tolerance
        {
            get => _Tolerance; 
            set
            {
                if (value >= 0)
                   SetProperty(ref _Tolerance , value);
            }
        }
        #endregion

        #region Is_Active - a property for knowing wheather the product has been deleted or not
        private bool _Is_Active;
        public bool Is_Active
        {
            get { return _Is_Active; }
            set { _Is_Active = value; }
        }
        #endregion

        #region Quantity Available

        private int _Quantity_Available;

        public int Quantity_Available
        {
            get => _Quantity_Available; 
            set => SetProperty(ref _Quantity_Available , value);
        }

        #endregion

        #region Created At
        private string _Created_At;
        public string Created_At
        {
            get { return _Created_At; }
            set { _Created_At = value; }
        }
        #endregion

        #region Constructor
        public Inventory(string productId, string productName, int quantityAvailable,double Tolerance, string date)
        {
            this.Product_ID = productId;
            this.Product_Name = productName;
            this.Quantity_Available = quantityAvailable;
            this.Tolerance = Tolerance;
            this.Created_At = date;
            this.Is_Active = true;
        }

        #endregion

        #region Default Constructor


        public Inventory()
        {
        }
        #endregion


    }
}
