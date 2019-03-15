using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Tower.Core.Services
{
    public abstract class BindableService : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void HandlePropertyChange<T>(ref T field, T value, [CallerMemberName] string propertyName = "") where T : IEquatable<T>
        {
            if (value.Equals(field)) return;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
