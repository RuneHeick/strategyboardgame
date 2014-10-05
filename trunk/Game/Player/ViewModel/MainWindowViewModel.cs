using Network.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedData.Types;
using System.ComponentModel;
using Utility.ViewModel;

namespace Player.ViewModel
{
    public class MainWindowViewModel: ViewModelBase
    {

        Dictionary<string, ViewModelBase> Views = new Dictionary<string, ViewModelBase>(); 

        ViewModelBase currentView_;
        public ViewModelBase CurrentView
        {
            get
            {
                return currentView_;
            }
            set
            {
                currentView_ = value;
                OnPropertyChanged("CurrentView");
            }
        }

        
        public MainWindowViewModel()
        {
            PlayerData.Instance.SwitchViewFunction = SwitchView; 
            Views.Add("Login", new LoginViewModel());
            Views.Add("Main", new MainViewModel());
            CurrentView = Views["Login"]; 
        }

        public void SwitchView(string View)
        {
            if(Views.ContainsKey(View))
            {
                CurrentView = Views[View];
            }
        }
       

    }
}
