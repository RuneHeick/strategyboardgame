using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.ViewModel;
using System.Collections.ObjectModel;
using Player.ViewModel.Tabs;
using Logic;
using SharedData.Types;

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


        public ObservableCollection<TagIntContainor> Resources 
        {
            get
            {
                return Rec.Rec; 
            }
        }

        private UserRec Rec; 

        public MainViewModel()
        {
            Tabs = new ObservableCollection<TabBase>();
            PlayerData.Instance.Client.OnDisconnect += Client_OnDisconnect;
            Tabs.Add(new MainTab());
            Tabs.Add(new BuildingsTab());
            Tabs.Add(new MyBuildingsTab());
            Rec = new UserRec(PlayerData.Instance.Client.dataManager); 
        }

        void Client_OnDisconnect(Network.Server.ClientData obj)
        {
            CreateCloseRequest();
        }

    }
}
