﻿using Logic;
using SharedData;
using SharedData.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;

namespace SharedLogic.Global
{
    [Serializable]
    public class UsersList:ISharedData
    {

        public ObservableCollection<CollectionItem> Users { get; private set; }

        public string Name
        {
            get
            {
                return "UserList";
            }
            set
            {
                throw new NotImplementedException(); 
            }
        }

        public UsersList()
        {
            Users = new ObservableCollection<CollectionItem>();
        }
        
        public void Increase(string Name)
        {
            lock (Users)
            {
                CollectionItem item = Users.FirstOrDefault((o) => o.Name == Name);
                if (item != null)
                {
                    item.Count++;
                }
                else
                {
                    Users.Add(new CollectionItem() { Name = Name, Count = 1 });
                }
            }
            OnPropertyChanged("Users");
        }

        public void Decrease(string Name)
        {
            lock (Users)
            {
                CollectionItem item = Users.FirstOrDefault((o) => o.Name == Name);
                if (item != null)
                {
                    item.Count--;
                }
            }
            OnPropertyChanged("Users");
        }

        public void Update(ISharedData data)
        {
            UsersList con = data as UsersList; 
            if(con != null)
            {
                foreach (CollectionItem item in con.Users)
                {
                    CollectionItem target = Users.FirstOrDefault((o)=>o.Name == item.Name); 
                    if(target != null)
                    {
                        if(target.Count != item.Count)
                            target.Count = item.Count; 
                    }
                    else
                    {
                        Users.Add(item); 
                    }
                }
            }
        }

        [Serializable]
        public class CollectionItem: INotifyPropertyChanged
        {
            private string name; 
            public string Name
            {
                get
                {
                    return name;
                }
                set
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }

            private int count; 
            public int Count
            {
                get
                {
                    return count; 
                }
                set
                {
                    count = value;
                    OnPropertyChanged("Count"); 
                }
            }

            [field: NonSerialized]
            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler handler = PropertyChanged;
                if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
