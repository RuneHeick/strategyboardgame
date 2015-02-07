using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coordinator.ViewModel.SelectedViewModel
{
    public class Border : SelectedBase
    {


        public string Image { get; set; }


        public Border()
        {
            Image = "border"; 
        }
        public override void DoAction(SharedData.DataManager manager, MainWindowViewModel mainVindow)
        {
            
        }

    }
}
