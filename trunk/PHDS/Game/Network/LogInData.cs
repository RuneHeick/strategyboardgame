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
    public class LogInData
    {
        public string Name { get; set; }
        public string Password { get; set; }

        public bool Create { get; set; }

        public LogInData()
        {
            Name = "";
            Password = "";
            Create = false; 
        }

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

                var item = a as LogInData;
                if (item != null)
                {
                    Password = item.Password;
                    Name = item.Name;
                    Create = item.Create; 
                }
            }
            catch
            {
                Password = "";
                Name = "";
            }
        }

    }
}
