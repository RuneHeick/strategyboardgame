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
    public class WarAttackRqSignal : SignalBase, INotifyPropertyChanged
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


        public string Defender { get; set; }

        public WarAttackRqSignal(int warCount)
        {
            ID = warCount; 
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
