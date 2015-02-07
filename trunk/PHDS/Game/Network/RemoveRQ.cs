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
    public class RemoveRQ
    {

        public string Name { get; set; }

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

                var item = a as RemoveRQ;
                if (item != null)
                {
                    Name = item.Name;
                }
            }
            catch
            {
                Name = "";
            }
        }

    }
}
