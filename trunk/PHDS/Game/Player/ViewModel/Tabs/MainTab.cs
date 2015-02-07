using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SharedData.Types;
using SharedLogic.Global;

namespace Player.ViewModel.Tabs
{
    public class MainTab:TabBase
    {
        public MainTab()
        {
            Name = "Main";
        }

        public ObservableCollection<UsersList.CollectionItem> Rec
        {
            get
            {
                return PlayerData.Instance.Users.Users;
            }
        }


    }
}
