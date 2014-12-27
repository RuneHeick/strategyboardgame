using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Types
{
    [Serializable]
    public class DoubleTagContainor:ISharedData
    {
        private double value_ = 0;

        public string Tag { get; private set; }

        public string RealName { get; private set; }

        public double Value
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

        public DoubleTagContainor(string Name, string Tag)
        {
            this.Name = Name+Tag;
            RealName = Name;
            this.Tag = Tag; 
        }

        public string Name { get; set;  }

        public void Update(ISharedData data)
        {
            DoubleTagContainor con = data as DoubleTagContainor; 
            if(con != null)
            {
                Value = con.value_;
            }
        }

        public override bool Equals(object obj)
        {
            ISharedData item = obj as ISharedData;
            if (item != null)
                return item.Name == Name;
            return base.Equals(obj); 
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
