using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals.War
{
    [Serializable]
    public class WarArmyContainor : INotifyPropertyChanged
    {

        public WarArmyContainor()
        { }

        public WarArmyContainor(WarArmyContainor old)
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

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
