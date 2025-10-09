using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            get => _Product_Name;
            set => SetProperty(ref _Product_Name, value);
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
                    SetProperty(ref _Tolerance, value);
            }
        }

        #endregion

        #region Is_Active - a property for knowing wheather the product has been deleted or not
        private bool _Is_Active;

        public bool Is_Active
        {
            get => _Is_Active; 
            set 
            {
                SetProperty(ref _Is_Active, value);
            }
        }
        #endregion

        #region In_Stock - a property for knowing if a product has enough quantity available or not
        private bool _In_Stock;
        public bool In_Stock
        {
            get => _In_Stock;
            set => SetProperty(ref _In_Stock, value);
        }
        #endregion

        #region Quantity Available
        private int _Quantity_Available;

        public int Quantity_Available
        {
            get => _Quantity_Available;
            set => SetProperty(ref _Quantity_Available, value);
        }
        #endregion

        #region Created At
        private string _Created_At;
        public string Created_At
        {
            get => _Created_At;
            set => SetProperty(ref _Created_At, value);
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
            this.In_Stock = true;
        }

        #endregion

        #region Default Constructor
        public Inventory()
        {
            this.Is_Active = true;
            this.In_Stock = true;
        }

        #endregion
    }
}
