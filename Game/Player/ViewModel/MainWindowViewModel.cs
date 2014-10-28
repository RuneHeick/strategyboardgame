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
        List<ViewModelBase> viewStack = new List<ViewModelBase>();         
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
            PlayerData.Instance.SwitchViewFunction = AddOnStack;
            AddOnStack(new LoginViewModel());
        }

        public void AddOnStack(ViewModelBase View)
        {
            if(View != null)
            {
                lock (viewStack)
                {
                    View.OnCloseRequest += View_OnCloseRequest;
                    CurrentView = View;
                    viewStack.Insert(0, View);
                }
            }
        }

        void View_OnCloseRequest(ViewModelBase View)
        {
            lock (viewStack)
            {
                viewStack.Remove(View);
                if(viewStack.Count>0)
                {
                    if (CurrentView != viewStack[0])
                        CurrentView = viewStack[0];
                }
                else
                {
                    CurrentView = null; 
                }
            }
            View.OnCloseRequest -= View_OnCloseRequest;
        }
       

    }
}
