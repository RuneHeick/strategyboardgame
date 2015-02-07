using SharedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coordinator.ViewModel.SelectedViewModel
{
    public abstract class SelectedBase
    {
        public string Name { get; set; }


        public abstract void DoAction(DataManager manager, MainWindowViewModel mainVindow);
        

    }
}
