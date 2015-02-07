using SharedLogic.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Utility.ViewModel;

namespace Player.ViewModel
{
    public class LoseViewModel:ViewModelBase
    {

        WarResultContaionor warres; 

        public LoseViewModel(WarResultContaionor res)
        {
            warres = res; 
        }


        public string Winner
        {
            get
            {
                return warres.Winner; 
            }
        }


        public string Loser
        {
            get
            {
                return warres.Loser; 
            }
        }

        private RelayCommand doneCommand;
        public ICommand DoneCommand
        {
            get
            {
                if (doneCommand == null)
                    doneCommand = new RelayCommand((p) => doneCommandExecute());
                return doneCommand;
            }
        }

        private void doneCommandExecute()
        {
            PlayerData.Instance.Manager.Remove(warres); 
            CreateCloseRequest(); 
        }

    }
}
