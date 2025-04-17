using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RestND.Helpers
{
    public class NotifyPropertyChangedBase : INotifyPropertyChanged
    {
        // An event that gets triggered when a property changes
        public event PropertyChangedEventHandler PropertyChanged;

        // A method to call when a property value changes
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // If someone is listening for property changes, notify them
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
