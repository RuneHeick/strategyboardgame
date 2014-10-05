using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SharedData
{
    public class DataManager
    {
        Dictionary<string, ISharedData> DataByName = new Dictionary<string, ISharedData>();
        UpdateQueue PendingNetworkUpdate = new UpdateQueue();

        bool hasUpdates = false;

        public string[] ItemNames
        {
            get
            {
                lock (DataByName)
                {
                    return DataByName.Keys.ToArray();
                }
            }
        }

        public T GetItem<T>(string Name)
        {
            if (DataByName.ContainsKey(Name))
            {
                ISharedData item = DataByName[Name];
                if(item != null)
                {
                    try
                    {
                        return (T)item;
                    }
                    catch
                    {
                        
                    }
                }
            }
            return default(T); 
        }

        public bool Add(ISharedData item)
        {
            if (item == null) return false; 
            lock (DataByName)
            {
                if (DataByName.ContainsKey(item.Name))
                {
                    return false;
                }

                DataByName.Add(item.Name, item);
                item.PropertyChanged += item_PropertyChanged;
                item_PropertyChanged(item, null);
                if (CollectionChanged != null)
                    CollectionChanged(item.Name, item, ChangeType.Added, this); 
                return true;
            }
        }

        public void Remove(ISharedData item)
        {
            if (item != null)
            {
                if (DataByName.ContainsKey(item.Name))
                {
                    item.PropertyChanged -= item_PropertyChanged;
                    if (ItemRemoved != null)
                        ItemRemoved(item);

                    DataByName.Remove(item.Name);

                    if (CollectionChanged != null)
                        CollectionChanged(item.Name, item, ChangeType.Removed, this);
                }
            }
        }

        void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            lock (PendingNetworkUpdate)
            {
                ISharedData item = sender as ISharedData;
                if (item != null)
                {
                    PendingNetworkUpdate.Enqueue(item);
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (Updates != null)
                        {
                            try
                            {
                                byte[] a = DoNetworkUpdate();
                                if(a != null)
                                    Updates(a);
                            }
                            catch (Exception t)
                            {

                            }
                        }
                    }));
                }
            }
        }

        public byte[] DoNetworkUpdate()
        {
            lock (PendingNetworkUpdate)
            {
                TypeList item = new TypeList();
                ISharedData data = PendingNetworkUpdate.Dequeue();
                while (data != null)
                {
                    item.items.Add(data);
                    data = PendingNetworkUpdate.Dequeue();
                }

                if (item.items.Count > 0)
                {
                    return item.getByte();
                }
                return null;
            }
        }

        public byte[] GetAllData()
        {
            lock (PendingNetworkUpdate)
            {
                lock (DataByName)
                {
                    TypeList item = new TypeList();
                    foreach (ISharedData data in DataByName.Values)
                    {
                            item.items.Add(data);
                    }

                    if (item.items.Count > 0)
                    {
                        return item.getByte();
                    }
                    return null;
                }
            }
        }

        public void UpdateFromNetwork(byte[] data)
        {
            try
            {
                TypeList item = new TypeList();
                item.setByte(data); 
                foreach(ISharedData tempitem in item.items)
                {
                    if(DataByName.ContainsKey(tempitem.Name))
                    {
                        DataByName[tempitem.Name].PropertyChanged -= item_PropertyChanged; 
                        DataByName[tempitem.Name].Update(tempitem);
                        DataByName[tempitem.Name].PropertyChanged += item_PropertyChanged; 
                    }
                    else
                    {
                        DataByName.Add(tempitem.Name, tempitem);
                        DataByName[tempitem.Name].PropertyChanged += item_PropertyChanged;
                        if (CollectionChanged != null)
                            CollectionChanged(tempitem.Name, tempitem, ChangeType.Added, this); 
                    }
                }
            }
            catch(Exception e)
            {
            }
        }

        public event Action<byte[]> Updates;
        public event Action<ISharedData> ItemRemoved; 

        public event Action<string,ISharedData, ChangeType, DataManager> CollectionChanged;


    }

    public enum ChangeType
    {
        Added,
        Removed
    }
}
