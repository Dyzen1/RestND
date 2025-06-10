using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestND.MVVM.ViewModel.Dishes
{
    public class SelectableItem<T> : ObservableObject
    {
        public T Value { get; }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public SelectableItem(T value)
        {
            Value = value;
        }
    }

}
