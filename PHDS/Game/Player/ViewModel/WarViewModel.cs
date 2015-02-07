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
    public class WarViewModel:ViewModelBase
    {
        WarContaionor Currentwar;  

        public WarViewModel(WarContaionor item, int maxSize)
        {
            Currentwar = item; 
            Attacker = new WarItem();
            Attacker.Item = new WarContaionor.CollectionItem(item.Attacker); 
            Attacker.ADLable = "Attack"; 
            if(Attacker.Item.Name == PlayerData.Instance.Client.LoginName)
            {
                Attacker.MaxSoldier = maxSize;
                Attacker.IsMe = true; 
            }
            else
            {
                Attacker.IsMe = false;
            }


            Defender = new WarItem();
            Defender.Item = new WarContaionor.CollectionItem(item.Defender); 
            Defender.ADLable = "Defence";
            if (Defender.Item.Name == PlayerData.Instance.Client.LoginName)
            {
                Defender.MaxSoldier = maxSize;
                Defender.IsMe = true;
            }
            else
            {
                Defender.IsMe = false;
                Defender.IsSelectable = true; 
            }
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
            if (Attacker.IsMe)
                Attacker.Item.IsDone = true;
            else
                Defender.Item.IsDone = true; 

            Currentwar.Defender = Defender.Item;
            Currentwar.Attacker = Attacker.Item; 
            OnClose(); 
        }

    }

    public class WarItem
    {
        
        public WarItem()
        {
            
        }

        public string[] Users
        { 
            get
            {
                List<string> temp = new List<string>(); 
                foreach(UsersList.CollectionItem User in PlayerData.Instance.Users.Users)
                {
                    temp.Add(User.Name); 
                }
                return temp.ToArray();
            }
        }

        public WarContaionor.CollectionItem Item { get; set; }

        public string ADLable { get; set; }

        public int MaxSoldier { get; set; }

        public bool IsMe { get; set; }

        public bool IsSelectable { get; set; }

    }
}
