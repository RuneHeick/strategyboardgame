using SharedLogic.Global;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Utility.ViewModel;
using Signals.War;
using Network;

namespace Player.ViewModel
{
    public class WarViewModel:ViewModelBase
    {
        SignalBase Currentwar;  

        public WarViewModel(WarAttackRqSignal item, int maxSize)
        {
            BaseInit(item);
            
            Attacker.ADLable = "Attack";
            Attacker.IsMe = true;
            Attacker.Item = item.Attacker;
            Attacker.MaxSoldier = maxSize;
            Attacker.IsSelectable = false;
            Attacker.Users = new string[] { PlayerData.Instance.Client.LoginName };

            Defender.IsSelectable = true;
            Defender.IsMe = false;
            Defender.ADLable = "";

            Defender.Item = new WarArmyContainor(); 

            List<string> namelist = new List<string>();
            try
            {
                foreach (UsersList.CollectionItem user in PlayerData.Instance.Users.Users)
                {
                    if (user.Name != PlayerData.Instance.Client.LoginName)
                        namelist.Add(user.Name);
                }
            }
            catch
            {

            }

            Defender.Users = namelist.ToArray();

        }

        public WarViewModel(WarDefenceRqSignal item, int maxSize)
        {
            BaseInit(item);
            Attacker.ADLable = "Attacker";
            Attacker.IsMe = false;
            Attacker.Item = item.Attacker;
            Attacker.IsSelectable = false;
            Attacker.Users = new string[] { item.Attacker.Name };

            Defender.IsSelectable = false;
            Defender.IsMe = true;
            Defender.Item = item.Defender;
            Defender.ADLable = "Defence";
            Defender.MaxSoldier = maxSize;
            Defender.Users = new string[] { item.Defender.Name };

        }

        private void BaseInit(SignalBase item)
        {
            Currentwar = item; 
            Attacker = new WarItem();
            Defender = new WarItem(); 
        }


        public WarItem Attacker { get; set; }

        public WarItem Defender { get; set; }


        public void OnClose()
        {
            CreateCloseRequest();
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
            var item = Currentwar as WarAttackRqSignal;
            if(item != null)
            {
                item.Defender = Defender.Item.Name; 
            }

            Currentwar.Done = true; 
            OnClose(); 
        }

    }

    public class WarItem
    {
        
        public WarItem()
        {
            
        }

        public string[] Users { get; set; }

        public WarArmyContainor Item { get; set; }

        public string ADLable { get; set; }

        public int MaxSoldier { get; set; }

        public bool IsMe { get; set; }

        public bool IsSelectable { get; set; }

    }
}
