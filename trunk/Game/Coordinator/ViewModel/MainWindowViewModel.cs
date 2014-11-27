﻿using System;
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
using Utility.ViewModel;
using System.Collections.ObjectModel; 

namespace Coordinator.ViewModel
{
    public class MainWindowViewModel : ViewModelBase
    {

        public GameServer Server;

        public RCLogic RCManager { get; set; }
        public ProLogic ProManager { get; set; }
        public ArmyLogic ArmyManager { get; set; }
        public ResearchLogic ResearchManager { get; set; }

        public OnlineManagerLogic OnlineManager { get; set; }

        public ProjectionViewModel Projection { get; private set; }

        ObservableCollection<ViewModelBase> Views = new ObservableCollection<ViewModelBase>(); 

        public MainWindowViewModel()
        {
            Server = new GameServer(5050);
            var RC = Server.RCContainor;


            RC.OnDataManagerAdded += RC_OnDataManagerAdded;
            Server.OnClientLogin += Server_OnClientLogin;
            Server.OnClientDisconneced += Server_OnClientDisconneced;


            ResearchManager = new ResearchLogic();
            RCManager = new RCLogic(ResearchManager);
            ProManager = new ProLogic(RCManager, ResearchManager);
            ArmyManager = new ArmyLogic();
            OnlineManager = new OnlineManagerLogic();

            


            // Views 

            Views.Add(new TurnViewModel(this));

            Projection = new ProjectionViewModel();
            View.ProjektionView view = new View.ProjektionView()
            {
                DataContext = Projection,
                Name = "Projection",
                Height = System.Windows.SystemParameters.PrimaryScreenHeight,
                Width = System.Windows.SystemParameters.PrimaryScreenWidth
            };
            view.Show(); 

        }

        void Server_OnClientDisconneced(ClientData obj)
        {
            OnlineManager.Disconnect(obj);
        }

        void RC_OnDataManagerAdded(string UserName, SharedData.DataManager obj)
        {
            RCManager.Create(UserName, obj);
            ProManager.Create(UserName, obj);
            ArmyManager.Create(UserName, obj);
            ResearchManager.Create(UserName, obj);
            OnlineManager.Create(UserName, obj);
        }

        private void Server_OnClientLogin(AcceptedArg obj)
        {
            if (obj.Create)
            {
                obj.IsAccepted = true;
                OnlineManager.Login(obj.Client);
            }
            else
            {
                if (Server.RCContainor.HasManager(obj.Client.LoginName, obj.Client.Password))
                {
                    obj.IsAccepted = true;
                    OnlineManager.Login(obj.Client);
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