using SharedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Network.Server
{
    public class ClientData
    {
        protected byte[] sizebuffer = new byte[3];
        protected byte[] Databuffer;
        int readindex = 0; 

        DataManager dataManager_;
        public DataManager dataManager
        {
            get
            {
                return dataManager_; 
            }
            set
            {
                if (dataManager_ != null)
                {
                    dataManager_.Updates -= OnNewUpdates;
                    dataManager_.CollectionChanged -= ChangeItemExistence;
                }
                dataManager_ = value;
                if (value != null)
                {
                    dataManager.Updates += OnNewUpdates;
                    dataManager_.CollectionChanged += ChangeItemExistence;
                }
            }
        }
     
        public string LoginName { get; set; }
        public string Password { get; set; }

        protected TcpClient handler;

        protected ClientData()
        { }

        public ClientData(TcpClient handler)
        {
            LoginName = "";
            Password = ""; 
            this.handler = handler;
            NetworkStream stream = handler.GetStream();
            stream.BeginRead(sizebuffer, 0, sizebuffer.Length, DataReceivedSize, stream);
        }


        #region Recive

        protected void DataReceivedSize(IAsyncResult result)
        {
            NetworkStream stream = (NetworkStream)result.AsyncState;
            try
            {
                int len = stream.EndRead(result);

                if(len != 0)
                {
                    readindex += len; 

                    if(readindex == sizebuffer.Length)
                    {
                        int size = sizebuffer[1] + (sizebuffer[2] << 8);
                        Databuffer = new byte[size];
                        readindex = 0;
                        stream.BeginRead(Databuffer, 0, Databuffer.Length, DataReceivedData, stream);
                    }
                    else
                    {
                        stream.BeginRead(sizebuffer, readindex, sizebuffer.Length - readindex, DataReceivedSize, stream);
                    }
                }
                else
                {
                    Disconnected();
                }
            }
            catch
            {
                Disconnected();
            }
        }

        private void DataReceivedData(IAsyncResult result)
        {
            NetworkStream stream = (NetworkStream)result.AsyncState;
            try
            {
                int len = stream.EndRead(result);

                if (len == 0)
                {
                    Disconnected();
                }
                else
                {
                    readindex += len; 
                    if(readindex == Databuffer.Length)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() => HandleEvent((NetworkCommands)sizebuffer[0], Databuffer)));
                        readindex = 0; 
                        stream.BeginRead(sizebuffer, 0, sizebuffer.Length, DataReceivedSize, stream);
                    }
                    else
                    {
                        stream.BeginRead(Databuffer, readindex, Databuffer.Length - readindex, DataReceivedData, stream);
                    }

                    
                }

            }
            catch(Exception e)
            {
                Disconnected();
            }

        }

        #endregion

        #region Send 
        public void Send(NetworkCommands commands,byte[] data)
        {
            if (handler.Connected == true)
            {
                if (data != null)
                {
                    byte[] startIndexer = new byte[] { (byte)commands, (byte)data.Length, (byte)((data.Length >> 8)) };
                    byte[] Sendbuffer = new byte[startIndexer.Length + data.Length];
                    Array.Copy(startIndexer, 0, Sendbuffer, 0, startIndexer.Length);
                    Array.Copy(data, 0, Sendbuffer, startIndexer.Length, data.Length);
                    NetworkStream stream = handler.GetStream();
                    stream.WriteAsync(Sendbuffer, 0, Sendbuffer.Length, new System.Threading.CancellationToken());
                }
            }
            else
            {
                Disconnected(); 
            }

        }



        #endregion 


        #region handler

        private void OnNewUpdates(byte[] data)
        {
            Send(NetworkCommands.Data, data); 
        }

        private void ChangeItemExistence(string arg1, ISharedData obj, ChangeType arg3, DataManager manager)
        {
            if(arg3 == ChangeType.Removed)
            {
                RemoveRQ remove = new RemoveRQ();
                remove.Name = obj.Name;

                Send(NetworkCommands.Remove, remove.ToByte());
            }
            else if(arg3 == ChangeType.Added)
            {
                var data = manager.GetAddedItem(); 
                if(data != null)
                    Send(NetworkCommands.Create, data); 
            }
        }


        private void HandleEvent(NetworkCommands EventType, byte[] data)
        {
            if ((EventType == NetworkCommands.Data || EventType == NetworkCommands.Create) && Password != "")
            {
                if(dataManager != null)
                {
                    try
                    {
                        dataManager.UpdateFromNetwork(data, EventType == NetworkCommands.Create);
                        if (OnUpdateData != null)
                            OnUpdateData(this, EventType, data);
                    }
                    catch
                    {
                        Disconnected(); 
                    }
                }
            }
            if(EventType == NetworkCommands.Login && Password == "")
            {
                LogInData login = new LogInData();
                login.fromByte(data);
                LoginName = login.Name;
                Password = login.Password;

                if (OnRemoteLogIn != null)
                    OnRemoteLogIn(this, login.Create); 
            }
            if(EventType == NetworkCommands.Remove)
            {
                RemoveRQ remove = new RemoveRQ();
                remove.fromByte(data);

                ISharedData item = dataManager.GetItem<ISharedData>(remove.Name);
                if (item != null)
                {
                    dataManager.CollectionChanged -= ChangeItemExistence;
                    dataManager.Remove(item);
                    dataManager.CollectionChanged += ChangeItemExistence;
                }
            }
            if(EventType == NetworkCommands.AcceptedLogin)
            {
                AcceptedLogin login = new AcceptedLogin();
                login.fromByte(data);

                LoginAccepted(login.Accepted); 
            }
        }

        protected virtual void LoginAccepted(bool isAccepted)
        {
            
        }

        void Disconnected()
        {
            if (OnDisconnect != null)
            {
                OnDisconnect(this);
                OnDisconnect = null; 
            }
        }

        public event Action<ClientData> OnDisconnect;
        public event Action<ClientData, bool> OnRemoteLogIn;
        public event Action<ClientData, NetworkCommands, byte[]> OnUpdateData; 

        #endregion 

    }

    public enum NetworkCommands
    {
        Login = 0,
        Data = 1,
        Remove = 2,
        AcceptedLogin = 3,
        Create = 4, 
    }

}
