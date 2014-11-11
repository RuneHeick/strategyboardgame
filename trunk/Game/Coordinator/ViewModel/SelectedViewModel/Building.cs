using SharedLogic.Production;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coordinator.ViewModel.SelectedViewModel
{
    public class Building : SelectedBase
    {

        public string Image { get; set; }
        public string RandomBuilding { get; set; }

        public Building()
        {
            Image = "houses";
        }

        public override void DoAction(SharedData.DataManager manager, MainWindowViewModel mainVindow)
        {
            BuildingInfoContainor Allbuildings = new BuildingInfoContainor("Buildings", int.MaxValue);
            Random ran = new Random(DateTime.Now.Millisecond);
            int index = ran.Next(0, Allbuildings.Buildings.Count);

            string name = Allbuildings.Buildings[index].Type;

            var building = mainVindow.ProManager.BuildingManager.GetBuilding(name, null);
            RandomBuilding = name; 

            if(building != null)
            {
                manager.Add(building); 
            }

        }


    }
}
