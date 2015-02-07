using SharedData.Types;
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
    public class WinViewModel : LoseViewModel
    {

        StringContainor MoveRq;//("BuildingMove");
        
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


        public WinViewModel(WarResultContaionor res):base(res)
        {
            MoveRq = PlayerData.Instance.Manager.GetItem<StringContainor>("BuildingMove");
            if(MoveRq != null)
                MoveRq.PropertyChanged += MoveRq_PropertyChanged;
        }

        void MoveRq_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Claim = MoveRq.Value; 
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
            MoveRq.Value = Claim;  
        }

    }
}
