using SharedData.Types;
using SharedLogic.Global;
using Signals.War;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Utility.ViewModel;

namespace Player.ViewModel
{
    public class WinViewModel : LoseViewModel
    {        
        private string claim; 
        public string Claim
        {
            get
            {
                return claim; 
            }
            set
            {
                claim = value;
                OnPropertyChanged("Claim");
            }
        }


        public WinViewModel(WarResultSignal res):base(res)
        {
            Claim = ""; 
        }
        
        private RelayCommand claimCommand;
        public ICommand ClaimCommand
        {
            get
            {
                if (claimCommand == null)
                    claimCommand = new RelayCommand((p) => claimCommandExecute());
                return claimCommand;
            }
        }

        private void claimCommandExecute()
        {
            if (Claim != "")
            {
                Signals.Building.MoveSignal moveSignal = new Signals.Building.MoveSignal() { Building = Claim };
                PlayerData.Instance.Client.dataManager.Signal.Send(moveSignal);
                Claim = "";
            }
        }

    }
}
