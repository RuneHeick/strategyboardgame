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
    public class WarResultContaionor:ISharedData
    {

        public string Name
        {
            get
            {
                return "WarRes"+ID;
            }
        }

        public string ID { get; set; }

        public WarResultContaionor(string id)
        {
            ID = id;
        }

        public void Update(ISharedData data)
        {
            WarResultContaionor con = data as WarResultContaionor;
            Winner = con.Winner;
            Loser = con.Loser; 
        }


        public string Winner { get; set; }

        public string Loser { get; set; }


        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
