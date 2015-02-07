using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SharedData.Types;
using Utility.ViewModel;
using System.Windows.Input;
using SharedLogic.Production;
using Coordinator.Logic.Building;

namespace Player.ViewModel.Tabs
{
    public class BuildingsTab:TabBase
    {
        

        public BuildingsTab()
        {
            Name = "Shop";
            BuildingInfoContainor b = PlayerData.Instance.Client.dataManager.GetItem<BuildingInfoContainor>("Buildings");
            if(b != null)
            {
                b.PropertyChanged += b_PropertyChanged;
            }
        }

        void b_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Buildings");
        }

        public BuildingInfo SelctedBuilding { get; set; }

        public List<BuildingInfo> Buildings
        {
            get
            {
                BuildingInfoContainor b = PlayerData.Instance.Client.dataManager.GetItem<BuildingInfoContainor>("Buildings");
                if (b != null)
                {
                    return b.Buildings;
                }
                return null; 
            }
        }

        private RelayCommand byeCommand;
        public ICommand ByeCommand
        {
            get
            {
                if (byeCommand == null)
                    byeCommand = new RelayCommand((p) => byeCommandExecute());
                return byeCommand;
            }
        }

        private void byeCommandExecute()
        {

            CreateBuildingRq newBuilding = new CreateBuildingRq(SelctedBuilding.Type, "Test1");
            PlayerData.Instance.Client.dataManager.Add(newBuilding); 
            
        }


    }
}
