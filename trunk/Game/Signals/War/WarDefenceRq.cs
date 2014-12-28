using Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals.War
{
    [Serializable]
    public class WarDefenceRqSignal :SignalBase, INotifyPropertyChanged
    {
        public int ID { get; set; }

        private WarArmyContainor attacker;
        public WarArmyContainor Attacker
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


        private WarArmyContainor defender;
        public WarArmyContainor Defender
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


        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        public WarDefenceRqSignal(int warCount)
        {
            ID = warCount; 
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
