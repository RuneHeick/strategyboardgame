using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.ViewModel;
using System.Collections.ObjectModel;
using Player.ViewModel.Tabs;

namespace Player.ViewModel
{
    public class MainViewModel: ViewModelBase
    {
        public string Name
        {
            get
            {
                return PlayerData.Instance.Client.LoginName;
            }
        }

        public ObservableCollection<TabBase> Tabs { get; private set; }


        public MainViewModel()
        {
            Tabs = new ObservableCollection<TabBase>();

            Tabs.Add(new MainTab()); 
        }

    }
}
