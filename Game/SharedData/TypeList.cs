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
        public List<ItemUpdate> items = new List<ItemUpdate>(); 

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

        public void Add(ISharedData data, ChangeType Type)
        {
            ItemUpdate olddata = items.FirstOrDefault((o)=>o.Item.Name == data.Name && o.Change == ChangeType.Changed);
            if (olddata != null)
                items.Remove(olddata);
            lock (items)
            items.Add(new ItemUpdate(data, Type));
        }

        [Serializable]
        public class ItemUpdate
        {
            public ItemUpdate(ISharedData data, ChangeType Type)
            {
                Item = data;
                Change = Type;
            }
            public ISharedData Item { get; set; }

            public ChangeType Change { get; set; }
        }

        internal void Add(ItemUpdate data)
        {
            lock (items)
                items.Add(data); 
        }
    }
}
