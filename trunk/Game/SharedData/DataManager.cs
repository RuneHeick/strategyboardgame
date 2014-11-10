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
        UpdateQueue<TypeList.ItemUpdate> UpdateItemsData = new UpdateQueue<TypeList.ItemUpdate>(); 

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
                        if(item is T)
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

                StartNetworkUpdate(item, ChangeType.Added);


                DataByName.Add(item.Name, item);
                item.PropertyChanged += item_PropertyChanged;
                if (CollectionChanged != null)
                    CollectionChanged(item.Name, item, ChangeType.Added, this); 
                return true;
            }
        }


        private bool IsUpdating = false; 
        private void StartNetworkUpdate(ISharedData item, ChangeType changeType)
        {
            if (Updates != null)
            {
                lock (UpdateItemsData)
                    UpdateItemsData.Enqueue(new TypeList.ItemUpdate(item, changeType));
                
                if(IsUpdating == false)
                {
                    IsUpdating = true; 
                    Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        if (Updates != null)
                        {
                            try
                            {
                                byte[] a = GetByteItems();
                                IsUpdating = false; 
                                if (a != null && Updates != null)
                                    Updates(a);
                            }
                            catch (Exception t)
                            {

                            }
                        }
                        else
                            IsUpdating = false; 
                    }));
                }
            }

        }

        private byte[] GetByteItems()
        {
            lock (UpdateItemsData)
            {
                TypeList item = new TypeList();
                TypeList.ItemUpdate data = UpdateItemsData.Dequeue();
                while (data != null)
                {
                    item.Add(data);
                    data = UpdateItemsData.Dequeue();
                }

                if (item.items.Count > 0)
                {
                    return item.getByte();
                }
                return null;
            }
        }

        public void Remove(ISharedData item)
        {
            if (item != null)
            {
                if (DataByName.ContainsKey(item.Name))
                {
                    item.PropertyChanged -= item_PropertyChanged;
                    lock (DataByName)
                        DataByName.Remove(item.Name);
                    StartNetworkUpdate(item, ChangeType.Removed); 
                    if (CollectionChanged != null)
                        CollectionChanged(item.Name, item, ChangeType.Removed, this);
                }
            }
        }

        void item_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
                ISharedData item = sender as ISharedData;
                if (item != null)
                {
                    StartNetworkUpdate(item, ChangeType.Changed);
                }
        }

        
        public byte[] GetAllData()
        {
            lock (UpdateItemsData)
            {
                lock (DataByName)
                {
                    TypeList item = new TypeList();
                    foreach (ISharedData data in DataByName.Values)
                    {
                            item.Add(data,ChangeType.Added);
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
                foreach(TypeList.ItemUpdate tempitem in item.items)
                {
                    if(DataByName.ContainsKey(tempitem.Item.Name) && tempitem.Change == ChangeType.Changed)
                    {
                        lock (DataByName)
                        {
                            DataByName[tempitem.Item.Name].PropertyChanged -= item_PropertyChanged;
                            DataByName[tempitem.Item.Name].Update(tempitem.Item);
                            if (DataByName.ContainsKey(tempitem.Item.Name))
                                DataByName[tempitem.Item.Name].PropertyChanged += item_PropertyChanged;
                        }
                    }
                    else if (tempitem.Change == ChangeType.Added)
                    {
                        lock (DataByName)
                        {
                            DataByName.Add(tempitem.Item.Name, tempitem.Item);
                            DataByName[tempitem.Item.Name].Update(tempitem.Item);
                            if (DataByName.ContainsKey(tempitem.Item.Name))
                                DataByName[tempitem.Item.Name].PropertyChanged += item_PropertyChanged;
                        }
                        if (CollectionChanged != null)
                            CollectionChanged(tempitem.Item.Name, tempitem.Item, ChangeType.Added, this); 
                    }
                    else if (DataByName.ContainsKey(tempitem.Item.Name) && tempitem.Change == ChangeType.Removed)
                    {
                        lock (DataByName)
                        {
                            DataByName[tempitem.Item.Name].PropertyChanged -= item_PropertyChanged;
                            DataByName.Remove(tempitem.Item.Name);
                        }

                        if (CollectionChanged != null)
                            CollectionChanged(tempitem.Item.Name, tempitem.Item, ChangeType.Removed, this); 
                    }
                }
            }
            catch(Exception e)
            {
            }
        }

        public event Action<byte[]> Updates;

        public event Action<string,ISharedData, ChangeType, DataManager> CollectionChanged;


    }

    [Serializable]
    public enum ChangeType
    {
        Added,
        Removed,
        Changed
    }
}
