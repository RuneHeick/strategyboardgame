using Network;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals
{

    [Serializable]
    public class TurnTokenSignal : SignalBase
    {

        public TurnTokenSignal(string TurnID)
        {
            ID = TurnID;
        }

        public string ID { get; private set; }

        public enum TurnAction
        {
            None,
            War,
            RandomResearch,
            RandomResources,
            RandomBuilding,
            BorderLand
        }

        private TurnAction selectedAction;
        public TurnAction SelectedAction
        {
            get
            {
                return selectedAction;
            }
            set
            {
                if (selectedAction != value)
                {
                    selectedAction = value;
                }
            }
        }



    }
}
