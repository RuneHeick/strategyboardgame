﻿using Network.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace Network
{
    public class SignalManager
    {
        UInt16 currentSignalID = 0;

        private Dictionary<UInt16, SignalsInfo> SendSignals = new Dictionary<ushort, SignalsInfo>();
        
        private Dictionary<Type, List<SubscriberInfo>> Subscribers = new Dictionary<Type, List<SubscriberInfo>>(); 

        public SignalManager()
        {
            

        }

        private void incrementCurrentSignalID()
        {
            do
            {
                currentSignalID++;
                if (currentSignalID == 0)
                    currentSignalID++;
            } while (SendSignals.ContainsKey(currentSignalID)); 

        }

        #region UserInput 

        public void Send<T>(T Signal, Action<T> CallBack = null) where T : SignalBase
        {
            UInt16 id = 0;
            if (CallBack != null)
            {
                lock (SendSignals)
                {
                    incrementCurrentSignalID();
                    id = currentSignalID;


                    SendSignals.Add(id, new SignalsInfo()
                    {
                        Function = (o) =>
                            {
                                T data = o as T;
                                if (data != null)
                                    CallBack(data);
                            },
                        Item = Signal
                    }); 
                }
            }

            byte[] idByte = BitConverter.GetBytes(id);
            byte[] signalByte = ToByte(Signal);
            byte[] packet = new byte[signalByte.Length + idByte.Length];
            Array.Copy(idByte, packet, idByte.Length);
            Array.Copy(signalByte, 0, packet, idByte.Length, signalByte.Length);

            FireOnSendReqest(NetworkCommands.Signal, packet); 
        }

        public void AddSignalHandler<T>(Action<T> Signal) where T: SignalBase
        {
            lock (Subscribers)
            {
                List<SubscriberInfo> list; 

                if (Subscribers.ContainsKey(typeof(T)))
                {
                    list = Subscribers[typeof(T)];
                }
                else
                {
                    list = new List<SubscriberInfo>();
                    Subscribers.Add(typeof(T), list);
                }

                if (list.FirstOrDefault((o) => o.OrginalFunction.Equals(Signal)) == null)
                {
                    SubscriberInfo item = new SubscriberInfo() { OrginalFunction = Signal };
                    item.CallFunction = (o) =>
                        {
                            T data = o as T;
                            if (data != null)
                                Signal(data);
                        };

                    list.Add(item);
                }

            }
        }

        public void RemoveSignalHandler<T>(Action<T> Signal) where T : SignalBase
        {
            lock (Subscribers)
            {
                if (Subscribers.ContainsKey(typeof(T)))
                {
                    var list = Subscribers[typeof(T)];
                    var item = list.FirstOrDefault((o) => o.OrginalFunction.Equals(Signal)); 
                    if (item != null)
                    {
                        list.Remove(item);
                    }
                }
            }
        }

        private void InvokeSignalHandler(SignalBase Signal)
        {
            Type id = Signal.GetType(); 
            if(Subscribers.ContainsKey(id))
            {
                lock (Subscribers)
                {
                    var list = Subscribers[id];
                    for(int i = 0; i<list.Count; i++)
                    {
                        var action = list[i] as SubscriberInfo;
                        action.CallFunction(Signal);
                    }
                }
            }
        }

        private class SubscriberInfo
        {
            public object OrginalFunction { get; set; }

            public Action<SignalBase> CallFunction { get; set; }

        }

        private class SignalsInfo
        {
            public Action<SignalBase> Function { get; set; }

            public SignalBase Item { get; set; }
        }

        #endregion 

        public void SignalRecived(byte[] Data,SignalType signalType )
        {
            byte[] packet = new byte[Data.Length - 2]; 
            Array.Copy(Data,2,packet,0,packet.Length);
            SignalBase item = FromByte(packet); 
            UInt16 id = BitConverter.ToUInt16(Data, 0);
            if (item != null)
            {
                if (signalType == SignalType.Response)
                    ResponseRecived(item, id);
                else if (signalType == SignalType.Signal)
                    NewSignalRecived(item, id);
            }
        }

        private void NewSignalRecived(SignalBase item, UInt16 id)
        {
            InvokeSignalHandler(item); 
            if(id != 0)
            {
                if(item.Done)
                {
                    sendResponse(item, id); 
                }
                else
                {
                    item.DoneChanged += (o) => item_DoneChanged(o, id);
                }
            }
        }

        void item_DoneChanged(SignalBase obj, UInt16 id)
        {
            if(obj.Done)
                sendResponse(obj, id); 
        }

        private void sendResponse(SignalBase signal, UInt16 id)
        {
            byte[] idByte = BitConverter.GetBytes(id);
            byte[] signalByte = ToByte(signal);
            byte[] packet = new byte[signalByte.Length + idByte.Length];
            Array.Copy(idByte, packet, idByte.Length);
            Array.Copy(signalByte, 0, packet, idByte.Length, signalByte.Length);
            FireOnSendReqest(NetworkCommands.SignalResponse, packet);
        }

        private void ResponseRecived(SignalBase item, UInt16 id)
        {
            lock(SendSignals)
            {
                if(SendSignals.ContainsKey(id))
                {
                    SendSignals[id].Function(item);
                    SendSignals.Remove(id); 
                }
            }
        }
        

        private SignalBase FromByte(byte[] data)
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(data);
            object a = formatter.Deserialize(stream);
            stream.Close();

            var item = a as SignalBase;
            if (item != null)
            {
                return item; 
            }
            return null; 
        }

        private byte[] ToByte(SignalBase data)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream();
                formatter.Serialize(stream, data);
                stream.Close();
                return stream.GetBuffer();
            }
            catch(Exception e)
            {
                return null; 
            }
        }

        public enum SignalType
        {
            Signal, 
            Response
        }

        private event Action<NetworkCommands, byte[]> sendRequest;
        public event Action<NetworkCommands,byte[]> SendRequest
        {
            add
            {
                SendUpdate(value);
                sendRequest += value;
            }
            remove
            {
                sendRequest -= value;
            }
        }

        private void SendUpdate(Action<NetworkCommands, byte[]> SignalManager_sendRequest)
        {
            lock (SendSignals)
            {
               
                foreach (ushort id in SendSignals.Keys)
                {
                    byte[] signalByte = ToByte(SendSignals[id].Item);
                    if (signalByte != null)
                    {
                        byte[] idByte = BitConverter.GetBytes(id);
                        byte[] packet = new byte[signalByte.Length + idByte.Length];
                        Array.Copy(idByte, packet, idByte.Length);
                        Array.Copy(signalByte, 0, packet, idByte.Length, signalByte.Length);

                        Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => SignalManager_sendRequest(NetworkCommands.Signal, packet)));
                    }
                }
            }
        }

        private void FireOnSendReqest(NetworkCommands commands,byte[] data)
        {
            if(sendRequest != null)
            {
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(()=>sendRequest(commands,data))); 
            }
        }

    }

    [Serializable]
    public abstract class SignalBase
    {
        private bool done = true; 
        public bool Done
        {
            get
            {
                return done; 
            }
            set
            {
                done = value;
                if (DoneChanged != null)
                    DoneChanged(this); 
            }
            
        }

        [field: NonSerialized]
        public event Action<SignalBase> DoneChanged; 
    }

}
