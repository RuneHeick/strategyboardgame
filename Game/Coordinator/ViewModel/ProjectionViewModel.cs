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
        
        private string currentTeam;
        public string CurrentTeam
        {
            get
            {
                return currentTeam;
            }
            set
            {
                currentTeam = value;
                OnPropertyChanged("CurrentTeam");
            }
        }

        private SelectedViewModel.SelectedBase selectedView;
        public SelectedViewModel.SelectedBase SelectedView 
        {
            get
            {
                return selectedView; 
            }
            set
            {
                selectedView = value;
                OnPropertyChanged("SelectedView");
            }
        }

        private string[] teams; 
        public string[] Teams
        {
            get
            {
                return teams;
            }
            set
            {
                teams = value;
                OnPropertyChanged("Teams");
            }
        }

        public PlotModel Model { get; private set;  }


        public ProjectionViewModel()
        {
            Model = new PlotModel("Game Stats");
            LineSeries line = new LineSeries("Test");
           
        }



        
    }
}
