using SharedData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Network.Server
{
    public class ClientData
    {
        protected byte[] sizebuffer = new byte[1+4];
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
                    dataManager.Signal.SendRequest -= Send;
                }
                dataManager_ = value;
                if (value != null)
                {
                    dataManager.Updates += OnNewUpdates;
                    dataManager.Signal.SendRequest += Send;
                }
            }
        }

        public string LoginName { get; set; }
        public string Password { get; set; }

        protected TcpClient handler;

        protected ClientData()
        {

        }

        public ClientData(TcpClient handler)
        {
            LoginName = "";
            Password = ""; 
            this.handler = handler; 
            handler.Client.BeginReceive(sizebuffer, 0, sizebuffer.Length, 0, DataReceivedSize, handler.Client);
        }


        #region Recive
        protected void DataReceivedSize(IAsyncResult result)
        {
            Socket stream = (Socket)result.AsyncState;
            SocketError Error; 
            try
            {
                int len = stream.EndReceive(result, out Error);

                if(len != 0 && Error == SocketError.Success)
                {
                    readindex += len;

                    if (IsKnownMessage(sizebuffer[0]))
                    {
                        if (readindex == sizebuffer.Length)
                        {
                            int size = sizebuffer[1] + (sizebuffer[2] << 8) + (sizebuffer[3] << 16) + (sizebuffer[4] << 24);
                            Databuffer = new byte[size];
                            readindex = 0;
                            stream.BeginReceive(Databuffer, 0, Databuffer.Length, 0, DataReceivedData, stream);
                        }
                        else
                        {
                            stream.BeginReceive(sizebuffer, readindex, sizebuffer.Length - readindex, 0, DataReceivedSize, stream);
                        }
                    }
                    else // Will hopfully never be called
                    {
                        if(stream.Available>0)
                        {
                            byte[] tempbuffet = new byte[stream.Available];
                            stream.Receive(tempbuffet); 
                        }
                        stream.BeginReceive(sizebuffer, 0, sizebuffer.Length, 0, DataReceivedSize, stream);
                    }
                }
                else
                {
                    Disconnected();
                }
            }
            catch(Exception e)
            {
                Disconnected();
            }
        }

        private bool IsKnownMessage(byte Command)
        {
            return Enum.IsDefined(typeof(NetworkCommands), (int)Command);
        }

        private void DataReceivedData(IAsyncResult result)
        {

            Socket stream = (Socket)result.AsyncState;
            SocketError Error; 
            try
            {
                int len = stream.EndReceive(result, out Error);
                
                if (len != 0 && Error == SocketError.Success)
                {
                    readindex += len;
                    if (readindex == Databuffer.Length)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() => HandleEvent((NetworkCommands)sizebuffer[0], Databuffer)));
                        readindex = 0;
                        stream.BeginReceive(sizebuffer, 0, sizebuffer.Length, 0, DataReceivedSize, stream);
                    }
                    else
                    {
                        stream.BeginReceive(Databuffer, readindex, Databuffer.Length - readindex, 0, DataReceivedData, stream);
                    }
                }
                else
                {
                    Disconnected();
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
                    try
                    {
                        byte[] startIndexer = new byte[] { (byte)commands, (byte)data.Length, (byte)((data.Length >> 8)), (byte)((data.Length >> 16)), (byte)((data.Length >> 24)) };
                        byte[] Sendbuffer = new byte[startIndexer.Length + data.Length];
                        Array.Copy(startIndexer, 0, Sendbuffer, 0, startIndexer.Length);
                        Array.Copy(data, 0, Sendbuffer, startIndexer.Length, data.Length);
                        NetworkStream stream = handler.GetStream();
                        stream.WriteAsync(Sendbuffer, 0, Sendbuffer.Length, new System.Threading.CancellationToken());
                    }
                    catch
                    {
                        Disconnected(); 
                    }
                }
            }
        }



        #endregion 

        #region handler

        private void OnNewUpdates(byte[] data)
        {
            Send(NetworkCommands.Data, data); 
        }

        private void HandleEvent(NetworkCommands EventType, byte[] data)
        {
            if (EventType == NetworkCommands.Data && Password != "")
            {
                if(dataManager != null)
                {
                    try
                    {
                        dataManager.UpdateFromNetwork(data);
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
                LoginName = login.Name.Trim();
                Password = login.Password.Trim();

                if (OnRemoteLogIn != null)
                    OnRemoteLogIn(this, login.Create); 
            }
            if(EventType == NetworkCommands.AcceptedLogin)
            {
                AcceptedLogin login = new AcceptedLogin();
                login.fromByte(data);

                LoginAccepted(login.Accepted); 
            }
            if (EventType == NetworkCommands.Signal)
            {
                dataManager.Signal.SignalRecived(data, SignalManager.SignalType.Signal);
            }
            if (EventType == NetworkCommands.SignalResponse)
            {
                dataManager.Signal.SignalRecived(data, SignalManager.SignalType.Response);
            }
        }

        protected virtual void LoginAccepted(bool isAccepted)
        {
            
        }

        volatile object DisconnectLock = new object(); 
        void Disconnected()
        {
            lock (DisconnectLock)
            {
                if (OnDisconnect != null)
                {
                    OnDisconnect(this);
                    OnDisconnect = null;
                }
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
        AcceptedLogin = 3,
        Signal = 4, 
        SignalResponse = 5
    }

}
