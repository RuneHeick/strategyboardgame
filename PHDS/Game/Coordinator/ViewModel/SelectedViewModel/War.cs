using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coordinator.ViewModel.SelectedViewModel
{
    public class War: SelectedBase
    {

        public string Image { get; set; }

        public War()
        {
            Image = "war"; 
        }

        public override void DoAction(SharedData.DataManager manager, MainWindowViewModel mainVindow)
        {
            mainVindow.ArmyManager.StartWar(manager); 
        }
    }
}
