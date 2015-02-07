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
        List<ViewInfo> viewStack = new List<ViewInfo>();         
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
            AddOnStack(new LoginViewModel(), ViewPrioity.Background);
        }

        public void AddOnStack(ViewModelBase View, ViewPrioity priority)
        {
            if(View != null)
            {
                lock (viewStack)
                {
                    View.OnCloseRequest += View_OnCloseRequest;
                    ViewInfo info = new ViewInfo(View, priority);
                    for (int i = 0; i < viewStack.Count;i++)
                    {
                        if(viewStack[i].Priority >= info.Priority)
                        {
                            viewStack.Insert(i, info);
                            if (i == 0)
                                CurrentView = info.View;
                            return; 
                        }
                    }

                    viewStack.Add(info); 
                    if(viewStack.Count == 1)
                        CurrentView = info.View;
                }
            }
        }

        void View_OnCloseRequest(ViewModelBase View)
        {
            lock (viewStack)
            {
                var viewInfo = viewStack.FirstOrDefault((o) => o.View == View);
                viewStack.Remove(viewInfo);
                if(viewStack.Count>0)
                {
                    if (CurrentView != viewStack[0].View)
                        CurrentView = viewStack[0].View;
                }
                else
                {
                    CurrentView = null; 
                }
            }
            View.OnCloseRequest -= View_OnCloseRequest;
        }
       
        private class ViewInfo
        {
            public ViewInfo(ViewModelBase view,ViewPrioity priority)
            {
                Priority = priority;
                View = view; 
            }

            public ViewPrioity Priority { get; set; }

            public ViewModelBase View { get; set; }
        }
    }

    public enum ViewPrioity
    {
        Top = 0, 
        Background = 255
    }


}
