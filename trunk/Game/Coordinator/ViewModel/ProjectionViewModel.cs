using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.ViewModel;
using OxyPlot;
using OxyPlot.Series; 

namespace Coordinator.ViewModel
{
    public class ProjectionViewModel : ViewModelBase
    {

        public PlotModel Model { get; private set;  }


        public ProjectionViewModel()
        {
            Model = new PlotModel("Game Stats");
            LineSeries line = new LineSeries("Test");
           
        }


    }
}
