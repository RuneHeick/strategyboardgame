using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.ViewModel;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.Axes; 
using System.Collections.ObjectModel;
using Coordinator.Logic;
using System.Windows.Threading;
using SharedData;
using SharedData.Types;
using System.ComponentModel;

namespace Coordinator.ViewModel
{
    public class ProjectionViewModel : ViewModelBase
    {

        private DispatcherTimer Timer = new DispatcherTimer();

        public ObservableCollection<CollectionItem> Stats {get; private set;} 

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
        private MainWindowViewModel Mainwindow; 
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
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(()=>Timer_Tick(null, null))); 
            }
        }

        public PlotModel Model { get; private set;  }

        public ProjectionViewModel(MainWindowViewModel mvvm)
        {
            this.Mainwindow = mvvm;
            Stats = new ObservableCollection<CollectionItem>(); 
            Model = new PlotModel("Game Stats");
            Model.Axes.Add(new DateTimeAxis());
            Model.Axes.Add(new LinearAxis());

            Timer.Tick += Timer_Tick;
            Timer.Interval = MagicNumbers.GAMEPOINT_SAMPLETIME;
            Timer.Start();
           
        }


        void Timer_Tick(object sender, EventArgs e)
        {
            for (int i = 0; i < Mainwindow.RCManager.managers.Count; i++)
            {
                var player = Stats.FirstOrDefault((o) => o.Manager == Mainwindow.RCManager.managers[i].manager);

                if(player == null)
                {
                    var client = Mainwindow.Server.Cliens.FirstOrDefault((o) => o.dataManager == Mainwindow.RCManager.managers[i].manager);
                    if (client == null)
                        continue;
                    player = new CollectionItem(client.LoginName);
                    player.Manager = Mainwindow.RCManager.managers[i].manager;
                    Model.Series.Add(player.Line); 
                    Stats.Add(player);
                }

                DoSample(player);

            }

            Model.InvalidatePlot(true);
            Model.ResetAllAxes();
        }

        private void DoSample(CollectionItem player)
        {
            var item = player.Manager.GetItem<TagIntContainor>("Game PointResources");
            player.Line.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, item.Value));
            player.OnPropertyChanged("Average");
        }


        

        
    }

    public class CollectionItem : INotifyPropertyChanged
    {
        public CollectionItem(string Name)
        {
            Line = new LineSeries(Name);
            
        }

        public DataManager Manager { get; set; }

        public LineSeries Line { get; set; }

        public double Average
        {
            get
            {
                double sum = 0;
                for (int i = 0; i < Line.Points.Count; i++)
                {
                    sum += Line.Points[i].Y;
                }
                return sum / Line.Points.Count;
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

    }
}
