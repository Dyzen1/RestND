using CommunityToolkit.Mvvm.ComponentModel;
using RestND.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.ViewModel.Main
{
   public partial class MainWindowViewModel : ObservableObject
   {
        private readonly SignalRServices _signalRServices;
        public MainWindowViewModel(SignalRServices signalRServices)
        {
            _signalRServices = signalRServices;
        }
    }
}
