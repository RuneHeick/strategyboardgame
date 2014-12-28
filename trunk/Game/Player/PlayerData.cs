using Network.Client;
using SharedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SharedData.Types;
using Utility.ViewModel;
using SharedLogic.Turn;
using Player.ViewModel;
using SharedLogic.Global;
using Signals;

namespace Player
{
    public sealed class PlayerData
    {
        private static volatile PlayerData instance;
        private static object syncRoot = new Object();

        public ObservableCollection<TagIntContainor> Resources { get; private set; }

        public UsersList Users { get; set; }

        public GameClient Client { get; set; }
        
        public DataManager Manager
        {
            get
            {
                return Client.dataManager; 
            }
        }

        public Action<ViewModelBase, ViewPrioity> SwitchViewFunction { get; set; }

        private PlayerData() 
        {
            SwitchViewFunction = null;
            Resources = new ObservableCollection<TagIntContainor>(); 
            // Client setup in connect;
        }

        public void SwitchView(ViewModelBase view, ViewPrioity priority)
        {
            if(SwitchViewFunction != null)
            {
                SwitchViewFunction(view, priority); 
            }
        }

        public void Connect(string ip, int port)
        {
            Client = new GameClient(ip, port);

            Init(Client.dataManager);

            Client.dataManager.CollectionChanged += dataManager_CollectionChanged;
            Client.OnDisconnect += Client_OnDisconnect;
        }

        private void Init(DataManager dataManager)
        {
            dataManager.Signal.AddSignalHandler<TurnTokenSignal>(HandelTurnView); 
        }

        private void HandelTurnView(TurnTokenSignal obj)
        {
            var item = new TurnViewModel(obj);
            SwitchView(item, ViewPrioity.Top);
        }


        void Client_OnDisconnect(Network.Server.ClientData obj)
        {
            Client.dataManager.CollectionChanged -= dataManager_CollectionChanged;
        }

        void dataManager_CollectionChanged(string Name, ISharedData item, ChangeType ctype, DataManager manager)
        {
            if (Name.Contains("Resources"))
            {
                TagIntContainor rc = item as TagIntContainor;
                if (rc != null && rc.Tag == "Resources")
                {
                    if (ctype == ChangeType.Added)
                    {
                        Resources.Add(rc);
                    }
                    else
                    {
                        Resources.Remove(rc);
                    }
                }

            }

            var user = item as UsersList;
            if (user != null)
            {
                if (ctype == ChangeType.Added)
                {
                    Users = user;
                }
            }
        }


        TurnViewModel TurnView; 

        public static PlayerData Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new PlayerData();
                    }
                }

                return instance;
            }
        }
    }
}
