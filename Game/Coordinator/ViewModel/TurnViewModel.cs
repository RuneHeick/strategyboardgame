using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.ViewModel;
using System.Collections.ObjectModel;
using SharedLogic.Turn;
using SharedData;
using System.Windows.Threading;
using Coordinator.ViewModel.SelectedViewModel;
using System.Threading;
using System.Windows.Input;
using Signals; 

namespace Coordinator.ViewModel
{
    public class TurnViewModel : ViewModelBase
    {
        private MainWindowViewModel mainWindowViewModel;

        public string[] Teams { get; private set; }

        public bool Pause { get; set; }

        private int currentIndex; 
        public int CurrentIndex
        {
            get
            {
                return currentIndex; 
            }
            set
            {
                currentIndex = value % Teams.Length;
                OnPropertyChanged("CurrentIndex");
                OnPropertyChanged("CurrentTurn");
            }
        }
        public string CurrentTeam
        {
            get
            {
                lock (Teams)
                    return Teams[CurrentIndex]; 
            }
        }

        private uint Turn = 0; 



        public TurnViewModel(MainWindowViewModel mainWindowViewModel)
        {
            Pause = false; 
            this.mainWindowViewModel = mainWindowViewModel;
            mainWindowViewModel.Server.RCContainor.OnDataManagerAdded += RCContainor_OnDataManagerAdded;
            Teams = mainWindowViewModel.Server.RCContainor.ResourceContainers;
        }

        void RCContainor_OnDataManagerAdded(string arg1, SharedData.DataManager arg2)
        {
            lock (Teams)
                Teams = mainWindowViewModel.Server.RCContainor.ResourceContainers;
            mainWindowViewModel.Projection.Teams = Teams;
            OnPropertyChanged("Teams");

            if (t == null)
            {
                t = new DispatcherTimer();
                t.Tick += (o, s) => Next();
                t.Interval = new TimeSpan(0, 0, 10);
                t.Start();
            }
            
        }

        DispatcherTimer t = null;

        public void Next()
        {
            t.Stop();
            if (mainWindowViewModel.Server.Cliens.Count > 0)
            {
                while (true)
                {
                    CurrentIndex++;
                    var manager = mainWindowViewModel.Server.RCContainor[CurrentTeam];
                    if (mainWindowViewModel.Server.Cliens.FirstOrDefault((o) => o.dataManager == manager) != null)
                    {
                        Turn++;
                        TurnTokenSignal item = new TurnTokenSignal(Turn.ToString()) { Done = false};
                        manager.Signal.Send<TurnTokenSignal>(item, TurnReplay);

                        mainWindowViewModel.Projection.CurrentTeam = CurrentTeam;
                        mainWindowViewModel.Projection.SelectedView = null;
                        break;
                    }
                }
            }
        }

        private void TurnReplay(TurnTokenSignal obj)
        {
            if (obj.ID == Turn.ToString())
            {
                t.Stop();
                var manager = mainWindowViewModel.Server.RCContainor[CurrentTeam];
                if (manager != null)
                {
                    DoTurnData(manager, obj);
                }

                if (!Pause)
                    t.Start();
            }
        }

        

        private RelayCommand nextCommand;
        public ICommand NextCommand
        {
            get
            {
                if (nextCommand == null)
                    nextCommand = new RelayCommand((p) => Next());
                return nextCommand;
            }
        }

        private void DoTurnData(DataManager manager, TurnTokenSignal item)
        {
            SelectedBase selected = null; 
            switch(item.SelectedAction)
            {
                case TurnTokenSignal.TurnAction.BorderLand:
                    selected = new Border(); 
                    break;

                case TurnTokenSignal.TurnAction.RandomBuilding:
                    selected = new SelectedViewModel.Building(); 
                    break;

                case TurnTokenSignal.TurnAction.RandomResearch:
                    selected = new SelectedViewModel.Research(); 
                    break;

                case TurnTokenSignal.TurnAction.RandomResources:
                    selected = new SelectedViewModel.Resources(); 
                    break;

                case TurnTokenSignal.TurnAction.War:
                    selected = new SelectedViewModel.War(); 
                    break;
            }

            if(selected != null)
            {
                selected.DoAction(manager, mainWindowViewModel);
                mainWindowViewModel.Projection.SelectedView = selected;
            }

        }

         



    }
}
