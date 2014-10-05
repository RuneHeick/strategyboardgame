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
        List<BuildingInfo> buildings = null; 

        public BuildingsTab()
        {
            Name = "Buildings";
            BuildingInfoContainor b = PlayerData.Instance.Client.dataManager.GetItem<BuildingInfoContainor>("Buildings");
            if(b != null)
            {
                buildings = b.Buildings; 
            }
        }

        public BuildingInfo SelctedBuilding { get; set; }

        public List<BuildingInfo> Buildings
        {
            get
            {
                return buildings; 
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
