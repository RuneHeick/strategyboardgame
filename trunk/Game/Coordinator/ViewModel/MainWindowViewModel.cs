using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Network.Server;
using SharedData.Types;
using Network;

namespace Coordinator.ViewModel
{
    public class MainWindowViewModel
    {

        GameServer Server;
        IntContainor Connected; 

        public MainWindowViewModel()
        {
            Server = new GameServer(5050);
            var RC = Server.RCContainor;


            Connected = new IntContainor("Connected");
            Connected.Value = 0;
            RC.OnDataManagerAdded += RC_OnDataManagerAdded;
            Server.OnClientLogin += Server_OnClientLogin;
            Server.OnClientDisconneced += Server_OnClientDisconneced;
        }


        void RC_OnDataManagerAdded(SharedData.DataManager obj)
        {
            obj.Add(Connected);
            TagIntContainor Water = new TagIntContainor("Water", "Resources");
            Water.Value = 50;
            obj.Add(Water);
        }

        private void Server_OnClientDisconneced(ClientData obj)
        {
            Connected.Value--; 
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
                    Connected.Value++;
                }

            }
        }


    }
}
