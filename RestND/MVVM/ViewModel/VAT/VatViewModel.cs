using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using RestND.Data;
using RestND.MVVM.Model;
using RestND.MVVM.Model.VAT;
using RestND.Validations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.ViewModel.VAT
{
    public partial class VatViewModel : ObservableObject
    {
        #region Services
        private readonly VATservices _vatServices = new VATservices();
        private readonly VatValidator _vatValidator = new();
        #endregion

        #region Observable Properties
        [ObservableProperty] private Vat currentRate;
        [ObservableProperty] private string newRateInput;
        [ObservableProperty] private string errorMessage;
        #endregion

        #region Constructor
        public VatViewModel()
        {
            _vatServices = new VATservices();
            CurrentRate = _vatServices.Get();
        }
        #endregion

        #region On Change
        //partial void OnSelectedVatChanged(Vat value)
        //{
        //    UpdateVatCommand.NotifyCanExecuteChanged();
        //}
        #endregion

        #region Update VAT Rate
        [RelayCommand]
        public void UpdateVat()
        {
            if(!_vatValidator.ValidRate(
                NewRateInput, 
                out double parsed, 
                out string err))
            {
                ErrorMessage = err;
                return;
            }
            CurrentRate = _vatServices.Get();
            if(CurrentRate.Percentage == parsed)
            {
                ErrorMessage = "Same VAT rate!";
                return;
            }

            bool success = _vatServices.UpdateRate(parsed);
            if (success)
            {
                // passed validation:
                ErrorMessage = string.Empty;
                CurrentRate = _vatServices.Get();
                NewRateInput = string.Empty;
            }
        }
        #endregion
    }
}
