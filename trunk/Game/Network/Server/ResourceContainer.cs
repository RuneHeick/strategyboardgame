using SharedData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network.Server
{
    public class ResourceContainer
    {
        Dictionary<string, ContainorItem> Items = new Dictionary<string, ContainorItem>();

        public string[] ResourceContainers
        {
            get
            {
                return Items.Keys.ToArray(); 
            }
        }

        public DataManager GetDataManager(string Name, string Password)
        {
            var item = GetContainorItem(Name, Password);
            if (item != null)
            {
                return item.Data;
            }
            return null; 
        }

        public event Action<string, DataManager> OnDataManagerAdded; 

        public DataManager this[string name]
        {
            get
            {
                if(Items.ContainsKey(name))
                {
                    return Items[name].Data; 
                }
                return null; 
            }
        }

        public SignalManager GetSignalManager(string Name, string Password)
        {
            var item = GetContainorItem(Name, Password); 
            if(item != null)
            {
                return item.Data.Signal; 
            }
            return null; 
        }

        private ContainorItem GetContainorItem(string Name, string Password)
        {
            if (Items.ContainsKey(Name))
            {
                ContainorItem item = Items[Name];
                if (item.Password == Password)
                {
                    return item;
                }
            }
            else
            {
                ContainorItem item = new ContainorItem()
                {
                    Password = Password,
                    Name = Name,
                    Data = new DataManager(),
                };

                Items.Add(Name, item);
                if (OnDataManagerAdded != null)
                    OnDataManagerAdded(Name, item.Data);
                return item;
            }

            return null; 
        }


        class ContainorItem
        {
            public DataManager Data { get; set; }
            public string Password { get; set;  }

            public string Name { get; set; }
        }

        public bool HasManager(string Name)
        {
            return Items.ContainsKey(Name);
           
        }

        public bool HasManager(string Name, string Password)
        {
            if(Items.ContainsKey(Name))
            {
                ContainorItem a = Items[Name];
                if (a.Password == Password)
                    return true; 
            }
            return false;
        }
    }
}
