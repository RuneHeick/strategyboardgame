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

namespace Player
{
    public sealed class PlayerData
    {
        private static volatile PlayerData instance;
        private static object syncRoot = new Object();

        public ObservableCollection<TagIntContainor> Resources { get; private set; }

        public GameClient Client { get; set; }
        
        public DataManager Manager
        {
            get
            {
                return Client.dataManager; 
            }
        }

        public Action<ViewModelBase> SwitchViewFunction { get; set; }

        private PlayerData() 
        {
            SwitchViewFunction = null;
            Resources = new ObservableCollection<TagIntContainor>(); 
            // Client setup in connect;
        }

        public void SwitchView(ViewModelBase view)
        {
            if(SwitchViewFunction != null)
            {
                SwitchViewFunction(view); 
            }
        }

        public void Connect(string ip, int port)
        {
            Client = new GameClient(ip, port);

            Client.dataManager.CollectionChanged += dataManager_CollectionChanged;
            Client.OnDisconnect += Client_OnDisconnect;
        }


        void Client_OnDisconnect(Network.Server.ClientData obj)
        {
            Client.dataManager.CollectionChanged -= dataManager_CollectionChanged;
        }

        void dataManager_CollectionChanged(string Name, ISharedData item, ChangeType ctype, DataManager manager)
        {
            if(Name.Contains("Resources"))
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


        }


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
