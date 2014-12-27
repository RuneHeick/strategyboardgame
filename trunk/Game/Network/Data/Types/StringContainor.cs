using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Types
{
    [Serializable]
    public class StringContainor : ISharedData
    {
        private string value_ = "";

        public string Value
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

        public StringContainor(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public void Update(ISharedData data)
        {
            StringContainor con = data as StringContainor;
            if (con != null)
            {
                Value = con.value_;
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public override bool Equals(object obj)
        {
            ISharedData item = obj as ISharedData;
            if (item != null)
                return item.Name == Name;
            return base.Equals(obj);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
