using CommunityToolkit.Mvvm.ComponentModel;
<<<<<<< HEAD
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
=======

>>>>>>> 552c4bd5460e2d717d68ad1a5f45076c3608055f

namespace RestND.MVVM.Model
{
    public class Inventory : ObservableObject
    {
        #region Product ID
<<<<<<< HEAD
=======

>>>>>>> 552c4bd5460e2d717d68ad1a5f45076c3608055f
        private string _Product_ID;

        public string Product_ID
        {
            get => _Product_ID;
            set => SetProperty(ref _Product_ID, value);
        }
<<<<<<< HEAD
=======


>>>>>>> 552c4bd5460e2d717d68ad1a5f45076c3608055f
        #endregion

        #region Product Name
        private string _Product_Name;

<<<<<<< HEAD
        public string Product_Name
        {
            get => _Product_Name;
            set => SetProperty(ref _Product_Name, value);
=======
        private string _Product_Name;

        public string Product_Name
        {
            get  => _Product_Name;
            set => SetProperty( ref _Product_Name , value); 
>>>>>>> 552c4bd5460e2d717d68ad1a5f45076c3608055f
        }
        #endregion

        #region Tolerance
        private double _Tolerance;

        public double Tolerance
        {
<<<<<<< HEAD
            get => _Tolerance;
            set
            {
                if (value >= 0)
                    SetProperty(ref _Tolerance, value);
=======
            get => _Tolerance; 
            set
            {
                if (value >= 0)
                   SetProperty(ref _Tolerance , value);
>>>>>>> 552c4bd5460e2d717d68ad1a5f45076c3608055f
            }
        }
        #endregion

        #region Is_Active - a property for knowing wheather the product has been deleted or not
        private bool _Is_Active;

        public bool Is_Active
        {
<<<<<<< HEAD
            get => _Is_Active; 
            set 
            {
                SetProperty(ref _Is_Active, value);
            }
=======
            get  => _Is_Active; 
            set => SetProperty(ref _Is_Active ,value); 
>>>>>>> 552c4bd5460e2d717d68ad1a5f45076c3608055f
        }
        #endregion

        #region Quantity Available
        private int _Quantity_Available;

        public int Quantity_Available
        {
<<<<<<< HEAD
            get => _Quantity_Available;
            set => SetProperty(ref _Quantity_Available, value);
=======
            get => _Quantity_Available; 
            set => SetProperty(ref _Quantity_Available , value);
>>>>>>> 552c4bd5460e2d717d68ad1a5f45076c3608055f
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
            this.Is_Active = true;
        }

        #endregion
    }
}
