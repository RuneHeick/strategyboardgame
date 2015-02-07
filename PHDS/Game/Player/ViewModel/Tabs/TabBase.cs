using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.ViewModel;

namespace Player.ViewModel.Tabs
{
    public class TabBase:ViewModelBase
    {
        public string Name { get; set; }

        private bool visible = true;
        public bool Visible 
        {
            get
            {
                return visible; 
            }
            set
            {
                visible = value;
                OnPropertyChanged("Visible");
            }
        } 

    }
}
