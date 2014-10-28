using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network.Server;
using SharedData.Types;
using Network;
using System.Windows.Media;
using Coordinator.Logic;
using Logic;

namespace Coordinator.ViewModel
{
    public class MainWindowViewModel
    {

        GameServer Server;

        RCLogic RCManager;  
        ProLogic ProManager;
        ArmyLogic ArmyManager;
        ResearchLogic ResearchManager; 
        public MainWindowViewModel()
        {
            Server = new GameServer(5050);
            var RC = Server.RCContainor;


            RC.OnDataManagerAdded += RC_OnDataManagerAdded;
            Server.OnClientLogin += Server_OnClientLogin;

            ResearchManager = new ResearchLogic();
            RCManager = new RCLogic(ResearchManager);
            ProManager = new ProLogic(RCManager, ResearchManager);
            ArmyManager = new ArmyLogic();
            
        }

        void RC_OnDataManagerAdded(string UserName, SharedData.DataManager obj)
        {
            RCManager.Create(UserName, obj);
            ProManager.Create(UserName, obj);
            ArmyManager.Create(UserName, obj);
            ResearchManager.Create(UserName, obj); 
        }

        private void Server_OnClientLogin(AcceptedArg obj)
        {
            if (obj.Create)
                obj.IsAccepted = true;
            else
            {
                if (Server.RCContainor.HasManager(obj.Client.LoginName, obj.Client.Password))
                {
                    obj.IsAccepted = true;
                }

            }
        }


        public ImageSource IS
        {
            get
            {
                return UserRec.GetImage("game point"); 
            }
        }

    }
}
