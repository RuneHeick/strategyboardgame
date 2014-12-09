using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SharedData.Types;

namespace Player.ViewModel.Tabs
{
    public class MyBuildingsTab : TabBase
    {

        public ObservableCollection<BuildingContainor> Buildings { get; private set; }


        public MyBuildingsTab()
        {
            Visible = false; 
            Name = "Buildings";
            Buildings = new ObservableCollection<BuildingContainor>();
            Buildings.CollectionChanged += Buildings_CollectionChanged;
            Init();
        }

        void Buildings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                BuildingContainor a = e.NewItems[0] as BuildingContainor;
                if(a != null)
                {
                    PlayerData.Instance.SwitchView(new NewBuildingViewModel(a), ViewPrioity.Top);
                }
            }

            Visible = Buildings.Count > 0 ? true : false; 
        }

        private void Init()
        {
            string[] names = PlayerData.Instance.Manager.ItemNames; 
            foreach(string name in names)
            {
                if(name.StartsWith("Fac"))
                {
                    BuildingContainor b = PlayerData.Instance.Manager.GetItem<BuildingContainor>(name);
                    if (b != null)
                    {
                        Buildings.Add(b);
                    }
                }
            }

            PlayerData.Instance.Manager.CollectionChanged += Manager_CollectionChanged;
        }

        void Manager_CollectionChanged(string arg1, SharedData.ISharedData arg2, SharedData.ChangeType arg3, SharedData.DataManager arg4)
        {
            var building = arg2 as BuildingContainor;
            if(building != null)
            {
                if(arg3 == SharedData.ChangeType.Added)
                {
                    Buildings.Add(building);
                }
                else if (arg3 == SharedData.ChangeType.Removed)
                {
                    Buildings.Remove(building);
                }
            }
        }

    }
}
