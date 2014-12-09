using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Types
{
    [Serializable]
    public class IntContainor:ISharedData
    {
        private int value_ = 0; 

        public int Value
        {
            get
            {
                return value_; 
            }
            set
            {
                value_ = value;
                OnPropertyChanged("Value");
            }
        }

        public IntContainor(string name)
        {
            Name = name; 
        }

        public string Name { get; set;  }

        public override bool Equals(object obj)
        {
            ISharedData item = obj as ISharedData;
            if (item != null)
                return item.Name == Name;
            return base.Equals(obj);
        }
        public void Update(ISharedData data)
        {
            IntContainor con = data as IntContainor; 
            if(con != null)
            {
                Value = con.value_;
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
