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
            OnPropertyChanged("Teams");

            if (t == null)
            {
                t = new DispatcherTimer();
                t.Tick += (o, s) => Next();
                t.Interval = new TimeSpan(0, 0, 15);
                t.Start();
            }
            
        }

        DispatcherTimer t = null;
 
        public void Next()
        {
            t.Stop();
            t.Start(); 
                
                if (CurrentToken != null)
                {
                    var man= mainWindowViewModel.Server.RCContainor[CurrentTeam];
                    CurrentToken.PropertyChanged -= item_PropertyChanged;
                    man.Remove(CurrentToken);
                }

                CurrentIndex++;
                var manager = mainWindowViewModel.Server.RCContainor[CurrentTeam];

                TurnTokenContainor item = new TurnTokenContainor();
                CurrentToken = item;
                CurrentToken.PropertyChanged += item_PropertyChanged;

                manager.Add(item); 
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

            Next(); 
        }

        private void DoTurnData(DataManager manager, TurnTokenContainor item)
        {
            
        }

         



    }
}
