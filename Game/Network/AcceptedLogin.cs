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
    [Serializable]
    public class AcceptedLogin
    {

        public bool Accepted { get; set; }

        public byte[] ToByte()
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, this);
            stream.Close();
            return stream.GetBuffer();
        }

        public void fromByte(byte[] data)
        {
            try
            {
                IFormatter formatter = new BinaryFormatter();
                MemoryStream stream = new MemoryStream(data);
                object a = formatter.Deserialize(stream);
                stream.Close();

                var item = a as AcceptedLogin;
                if (item != null)
                {
                    Accepted = item.Accepted;
                }
            }
            catch
            {
                Accepted = false; 
            }
        }

    }
}
