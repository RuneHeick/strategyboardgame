using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace SharedData
{
    [Serializable]
    class TypeList
    {
        public List<ISharedData> items = new List<ISharedData>(); 

        public byte[] getByte()
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            formatter.Serialize(stream, this);
            stream.Close();
            return stream.GetBuffer();
        }

        public void setByte(byte[] data) 
        {
            IFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream(data);
            object a = formatter.Deserialize(stream);
            stream.Close();

            var item = a as TypeList; 
            if(item != null)
            {
                items = item.items; 
            }
        }

    }
}
