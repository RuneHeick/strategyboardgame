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

namespace Coordinator.ViewModel
{
    public class TurnViewModel : ViewModelBase
    {
        private MainWindowViewModel mainWindowViewModel;

        public string[] Teams { get; private set; }


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


        TurnTokenContainor CurrentToken = null; 



        public TurnViewModel(MainWindowViewModel mainWindowViewModel)
        {
            // TODO: Complete member initialization
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
                t.Interval = new TimeSpan(0, 0, 5);
                t.Start();
            }
            
        }

        DispatcherTimer t = null;

        public void Next()
        {
            t.Stop();
            if (mainWindowViewModel.Server.Cliens.Count > 0)
            {
                if (CurrentToken != null)
                {
                    var man = mainWindowViewModel.Server.RCContainor[CurrentTeam];
                    CurrentToken.PropertyChanged -= item_PropertyChanged;
                    man.Remove(CurrentToken);
                }

                while (true)
                {
                    CurrentIndex++;
                    var manager = mainWindowViewModel.Server.RCContainor[CurrentTeam];
                    if (mainWindowViewModel.Server.Cliens.FirstOrDefault((o) => o.dataManager == manager) != null)
                    {
                        TurnTokenContainor item = new TurnTokenContainor();
                        CurrentToken = item;
                        CurrentToken.PropertyChanged += item_PropertyChanged;

                        manager.Add(item);
                        mainWindowViewModel.Projection.CurrentTeam = CurrentTeam;
                        mainWindowViewModel.Projection.SelectedView = null;
                        break;
                    }
                }
            }
        }

        void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            t.Stop();
            var manager = mainWindowViewModel.Server.RCContainor[CurrentTeam];

            TurnTokenContainor item = sender as TurnTokenContainor;
            if(item != null)
            {
                DoTurnData(manager, item);
            }

            t.Start(); 
        }

        private void DoTurnData(DataManager manager, TurnTokenContainor item)
        {
            SelectedBase selected = null; 
            switch(item.SelectedAction)
            {
                case TurnTokenContainor.TurnAction.BorderLand:
                    selected = new Border(); 
                    break;

                case TurnTokenContainor.TurnAction.RandomBuilding:
                    selected = new SelectedViewModel.Building(); 
                    break;

                case TurnTokenContainor.TurnAction.RandomResearch:
                    selected = new SelectedViewModel.Research(); 
                    break;

                case TurnTokenContainor.TurnAction.RandomResources:
                    selected = new SelectedViewModel.Resources(); 
                    break;

                case TurnTokenContainor.TurnAction.War:
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
