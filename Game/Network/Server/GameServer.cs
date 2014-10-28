using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SharedData;

namespace Network.Server
{
    public class GameServer
    {
        public TcpListener ListenSocket { get; set; }

        ObservableCollection<ClientData> clients = new ObservableCollection<ClientData>();

        public ObservableCollection<ClientData> Cliens
        {
            get
            {
                return clients;
            }
        }

        public ResourceContainer RCContainor { get; private set;  }

        public GameServer(int port)
        {
            ListenSocket = new TcpListener(IPAddress.Any,port);
            ListenSocket.Start();
            ListenSocket.BeginAcceptTcpClient(AcceptConnection, ListenSocket);
            RCContainor = new ResourceContainer(); 
        }

        private void AcceptConnection(IAsyncResult ar)
        {
            TcpListener listener = (TcpListener)ar.AsyncState;
            TcpClient handler = listener.EndAcceptTcpClient(ar);

            AddNewSocket(handler);

            listener.BeginAcceptSocket(AcceptConnection, ListenSocket);
        }

        private void AddNewSocket(TcpClient handler)
        {
            
                ClientData data = new ClientData(handler);
                data.OnDisconnect += OnDisconnect;
                data.OnRemoteLogIn += OnLogin;
                data.OnUpdateData += OnUpdateData;
                lock (clients)
                {
                    clients.Add(data);
                }
                if (OnClientConnected != null)
                    OnClientConnected(data); 
        }

        private void OnUpdateData(ClientData client, NetworkCommands eventtype ,byte[] data)
        {
            lock (clients)
            {
                foreach(ClientData c in clients)
                {
                    if(c.dataManager == client.dataManager && c != client)
                    {
                        c.Send(eventtype, data); 
                    }
                }
            }
        }

        private void OnDisconnect(ClientData client)
        {
            client.OnDisconnect -= OnDisconnect;
            client.OnRemoteLogIn -= OnLogin;
            client.OnUpdateData -= OnUpdateData;
            lock (clients)
            {
                clients.Remove(client);
            }
            if (OnClientDisconneced != null)
                OnClientDisconneced(client);
        }

        private void OnLogin(ClientData client, bool Create)
        {
            if(client.LoginName != "" && client.Password != "")
            {

                AcceptedArg arg = new AcceptedArg(client);
                arg.Create = Create; 
                if (OnClientLogin != null)
                    OnClientLogin(arg);

                if (arg.IsAccepted)
                {
                    DataManager manager = RCContainor.GetDataManager(client.LoginName, client.Password);
                    if (manager != null)
                    {
                        client.dataManager = manager;

                        byte[] data = manager.GetAllData();

                        if (data != null)
                            client.Send(NetworkCommands.Create, data);
                    }
                    else
                    {
                        arg.IsAccepted = false; 
                    }
                }

                LoginEnd(arg); 
            }
        }

        protected void LoginEnd(AcceptedArg arg)
        {
            AcceptedLogin login = new AcceptedLogin(); 
            login.Accepted = arg.IsAccepted;
            arg.Client.Send(NetworkCommands.AcceptedLogin, login.ToByte()); 
        }

        public event Action<ClientData> OnClientConnected;
        public event Action<AcceptedArg> OnClientLogin;
        public event Action<ClientData> OnClientDisconneced;
    }
}
