using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Windows;

namespace Utility.ViewModel
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {

        protected ViewModelBase()
        {
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

        public void CreateCloseRequest()
        {
            if (OnCloseRequest != null)
                OnCloseRequest(this);
        }

        public event Action<ViewModelBase> OnCloseRequest; 


    }
}
