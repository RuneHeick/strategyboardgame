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
    public class SchoolContainor:BuildingContainor
    {

        public List<ResearchItem> ResearchQueue {get; set;}
       

        public SchoolContainor(string Type, string Id, List<UseCond> Uses, UseCond creates, UseCond creationBonus = null):base(Type,Id,Uses,creates,creationBonus)
        {
            ResearchQueue = new List<ResearchItem>();

            for (int i = 0; i < 5; i++)
            {
                var item = new ResearchItem("default", "none");
                item.Updated += SchoolContainor_PropertyChanged;
                ResearchQueue.Add(item);
            }
            
        }

        void SchoolContainor_PropertyChanged()
        {
            OnPropertyChanged("ResearchQueue");
        }

        public override void Update(ISharedData data)
        {
            base.Update(data);
            SchoolContainor school = data as SchoolContainor; 
            if(data != null)
            {
                for(int i = 0; i<school.ResearchQueue.Count; i++)
                {
                    ResearchQueue[i].Updated -= SchoolContainor_PropertyChanged;
                    ResearchQueue[i].Name = school.ResearchQueue[i].Name;
                    ResearchQueue[i].Description = school.ResearchQueue[i].Description;
                    ResearchQueue[i].Updated += SchoolContainor_PropertyChanged;
                }

            }
        }

    }

    [Serializable]
    public class ResearchItem : INotifyPropertyChanged
    {
        public ResearchItem(string name, string description)
        {
            Name = name;
            Description = description;
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

        private string decription; 
        public string Description
        {
            get
            {
                return decription;
            }
            set
            {
                decription = value;
                OnPropertyChanged("Description");
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        [field: NonSerialized]
        public event Action Updated;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);

                if (Updated != null)
                    Updated(); 

            }
        }
    }
}
