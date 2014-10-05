using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SharedData.Types;

namespace Player.ViewModel.Tabs
{
    public class MainTab:TabBase
    {
        public MainTab()
        {
            Name = "Main";
        }

        public ObservableCollection<TagIntContainor> Rec
        {
            get
            {
                return PlayerData.Instance.Resources;
            }
        }


    }
}
