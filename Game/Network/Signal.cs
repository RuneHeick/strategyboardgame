using Network.Server;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    public class SignalManager
    {

        private ClientData parrent;
        UInt16 currentSignalID = 0; 

        public SignalManager(ClientData Parrent)
        {
            this.parrent = Parrent; 

        }

        private void incrementCurrentSignalID()
        {
            currentSignalID++;
            if (currentSignalID == 0)
                currentSignalID++;
        }

        public void Send(SignalBase Signal, Action<SignalBase> CallBack = null)
        {



        }

        public void SignalRecived(byte[] Data,SignalType signalType )
        {
            SignalBase item = FromByte(Data);
            if (item != null)
            {
                if (signalType == SignalType.Response)
                    ResponseRecived(item);
                else if (signalType == SignalType.Signal)
                    NewSignalRecived(item);
            }
        }

        private void NewSignalRecived(SignalBase item)
        {
            throw new NotImplementedException();
        }

        private void ResponseRecived(SignalBase item)
        {
            throw new NotImplementedException();
        }
        

        public event Action<SignalBase> SignalRecived; 

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
            catch
            {
                return null; 
            }
        }

        public enum SignalType
        {
            Signal, 
            Response
        }


    }

    [Serializable]
    public abstract class SignalBase
    {
        
    }

}
