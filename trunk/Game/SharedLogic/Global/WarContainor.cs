using Logic;
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
    public class WarContaionor:ISharedData
    {

        public string Name
        {
            get
            {
                return "War"+ID;
            }
        }

        public string ID { get; set; }

        public WarContaionor(int id)
        {
            ID = id.ToString();
        }

        public void Update(ISharedData data)
        {
            WarContaionor con = data as WarContaionor; 
            if(con != null)
            {
                if (con.Attacker != null)
                {
                    if (Attacker != null)
                    {
                        Attacker.Alive = con.Attacker.Alive;
                        Attacker.WarSkill = con.Attacker.WarSkill;
                        Attacker.Name = con.Attacker.Name;
                        Attacker.IsDone = con.Attacker.IsDone;
                    }
                    else
                    {
                        Attacker = con.Attacker; 
                    }
                    OnPropertyChanged("Attacker");
                }
                if (con.Defender != null)
                {
                    if (Defender != null)
                    {
                        Defender.Alive = con.Defender.Alive;
                        Defender.WarSkill = con.Defender.WarSkill;
                        Defender.Name = con.Defender.Name;
                        Defender.IsDone = con.Defender.IsDone;
                    }
                    else
                    {
                        Defender = con.Defender;
                    }
                    OnPropertyChanged("Defender");
                }

            }
        }

        private CollectionItem attacker;
        public CollectionItem Attacker
        {
            get
            {
                return attacker; 
            }
            set
            {
                attacker = value;
                OnPropertyChanged("Attacker");
            }
        }


        private CollectionItem defender;
        public CollectionItem Defender
        {
            get
            {
                return defender;
            }
            set
            {
                defender = value;
                OnPropertyChanged("Defender");
            }
        }




        [Serializable]
        public class CollectionItem: INotifyPropertyChanged
        {

            public CollectionItem()
            {}

            public CollectionItem(CollectionItem old)
            {
                if (old != null)
                {
                    Name = old.Name;
                    Alive = old.Alive;
                    WarSkill = old.WarSkill;
                }
            }

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

            private int alive; 
            public int Alive
            {
                get
                {
                    return alive; 
                }
                set
                {
                    alive = value;
                    OnPropertyChanged("Alive"); 
                }
            }

            private int warSkill;
            public int WarSkill
            {
                get
                {
                    return warSkill;
                }
                set
                {
                    warSkill = value;
                    OnPropertyChanged("WarSkill");
                }
            }

            private bool isDone;
            public bool IsDone
            {
                get
                {
                    return isDone;
                }
                set
                {
                    isDone = value;
                    OnPropertyChanged("IsDone");
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
