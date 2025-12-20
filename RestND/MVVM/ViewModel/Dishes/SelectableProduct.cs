using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace RestND.MVVM.ViewModel.Dishes
{
    // A class that is used for multiple choise selections.
    public partial class SelectableProduct : ObservableObject
    {
        public string Product_ID { get; }
        public string Product_Name { get; }
        public double Quantity_Available { get; }

        [ObservableProperty] public bool isSelected;
        [ObservableProperty] public double amountUsage;

        public SelectableProduct(string id, string name, double qtyAvailable)
        {
            Product_ID = id;
            Product_Name = name;
            Quantity_Available = qtyAvailable;
            isSelected = false;
            amountUsage = 0;
        }
    }
}