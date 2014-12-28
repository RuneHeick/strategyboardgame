using SharedLogic.Turn;
using Signals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Utility.ViewModel;

namespace Player.ViewModel
{
    public class TurnViewModel: ViewModelBase
    {
        TurnTokenSignal turnInfo;

        public TurnViewModel(TurnTokenSignal TurnInfo)
        {
            turnInfo = TurnInfo; 
        }

        private RelayCommand selectCommand;
        public ICommand SelectCommand
        {
            get
            {
                if (selectCommand == null)
                    selectCommand = new RelayCommand((p) => closeCommandExecute(p));
                return selectCommand;
            }
        }

        private void closeCommandExecute(object obj)
        {
            string action = obj as string;
            if (action != null)
            {
                foreach (TurnTokenSignal.TurnAction Taction in Enum.GetValues(typeof(TurnTokenSignal.TurnAction)).Cast<TurnTokenSignal.TurnAction>())
                {
                    if(Enum.GetName(Taction.GetType(), Taction) == action)
                    {
                        turnInfo.SelectedAction = Taction;
                        break; 
                    }

                }
            }
            turnInfo.Done = true; 
            CreateCloseRequest();
        }


    }
}
