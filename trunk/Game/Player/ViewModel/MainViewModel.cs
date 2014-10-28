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
        private List<TabBase> AllTabs = new List<TabBase>(); 


        public ObservableCollection<TagIntContainor> Resources 
        {
            get
            {
                return Rec.Rec; 
            }
        }

        private UserRec Rec;

        public TabBase CurrentTab { get; set;  }

        public MainViewModel()
        {
            Tabs = new ObservableCollection<TabBase>();
            PlayerData.Instance.Client.OnDisconnect += Client_OnDisconnect;
            addTab(new MainTab());
            addTab(new BuildingsTab());
            var b = new MyBuildingsTab();
            addTab(b);
            addTab(new WarTab(b));
            addTab(new ResearchTab());
            Rec = new UserRec(PlayerData.Instance.Client.dataManager); 
        }

        public void addTab(TabBase item)
        {
            if (!AllTabs.Contains(item))
            {
                AllTabs.Add(item);
                item.PropertyChanged += item_PropertyChanged;
                if (item.Visible)
                    Tabs.Add(item);
            }
        }

        void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Visible")
            {
                TabBase item = sender as TabBase;
                if (item != null)
                {
                    if (item.Visible && (!Tabs.Contains(item)))
                        Tabs.Add(item); 
                    if ((!item.Visible) && Tabs.Contains(item))
                        Tabs.Remove(item); 
                }
            }
        }

        void Client_OnDisconnect(Network.Server.ClientData obj)
        {
            CreateCloseRequest();
        }

    }
}
