using SharedData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLogic.Turn
{

    [Serializable]
    public class TurnTokenContainor : ISharedData
    {

        public enum TurnAction
        {
            None,
            War,
            RandomResearch,
            RandomResources,
            RandomBuilding,
            BorderLand
        }

        public string Name
        {
            get
            {
                return "GameToken";
            }
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
                    OnPropertyChanged("SelectedAction");
                }
            }
        }

        public void Update(ISharedData data)
        {
            TurnTokenContainor con = data as TurnTokenContainor;
            if (con != null)
            {
                SelectedAction = con.SelectedAction; 
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
