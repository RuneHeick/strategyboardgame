using Network.Server;
using SharedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Network.Client
{
    public class GameClient : Server.ClientData
    {
        public const int ReconnectTry = 3;

        string ip;
        int port;

        public GameClient(string ip, int port)
        {
            this.ip = ip;
            this.port = port;
            dataManager = new DataManager();
            Connect();
        }

        private void Connect()
        {
            LoginName = "";
            Password = "";
            if (handler != null)
                Disconnect();
            handler = new TcpClient();
            handler.Connect(ip, port);

            NetworkStream stream = handler.GetStream();
            stream.BeginRead(sizebuffer, 0, sizebuffer.Length, DataReceivedSize, stream);
        }

        public void Login(string Name, string Password, bool Create = false)
        {
            if (Name != "" && Password != "")
            {
                LogInData login = new LogInData();
                login.Name = Name;
                login.Password = Password;
                login.Create = Create;
                if (dataManager == null) // not if reconnect 
                    dataManager = new DataManager();
                this.LoginName = Name;
                this.Password = Password;
                Send(NetworkCommands.Login, login.ToByte());
            }
        }

        public void Disconnect()
        {
            if (handler != null && handler.Connected)
                handler.Close();
        }

        protected override void LoginAccepted(bool isAccepted)
        {
            if(!isAccepted)
            {
                LoginName = "";
                Password = "";
            }

            if (OnLoginComplete != null)
                OnLoginComplete(isAccepted); 
        }


        public event Action<bool> OnLoginComplete; 

    }
}
