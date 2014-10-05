using Logic;
using SharedData;
using SharedData.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLogic.Production
{
    [Serializable]
    public class CreateBuildingRq:ISharedData
    {
        public string Id { get; set; } // unik id; 
        public string Name
        {
            get
            {
                return "RQFac" + Type + Id;
            }
            set
            {
                throw new NotImplementedException(); 
            }
        }

        public string Type { get; set; }

        public CreateBuildingRq(string Type, string Id)
        {
            this.Type = Type;
            this.Id = Id;
        }
        
        public void Update(ISharedData data)
        {
            CreateBuildingRq con = data as CreateBuildingRq; 
            if(con != null)
            {
                
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
