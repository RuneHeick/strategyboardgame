using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Logic; 

namespace SharedData.Types
{
    [Serializable]
    public class BuildingContainor:ISharedData
    {
        public string Id { get; set; } // unik id; 
        public string Name
        {
            get
            {
                return "Fac" + Name + Id;
            }
            set
            {
                throw new NotImplementedException(); 
            }
        }

        public string Type { get; set; }

        private UseCond production = null;
        public UseCond ProductionType
        {
            get
            {
                return production;
            }
            set
            {
                if(production != null)
                    production.PropertyChanged -= creates_PropertyChanged;
                production = value;
                if (production != null)
                    production.PropertyChanged += creates_PropertyChanged;
                OnPropertyChanged("Production");
            }
        }

        public UseCond CreationBonus { get; set; }

        private bool isActive;
        public bool IsActive
        {
            get
            {
                return isActive;
            }
            set
            {
                isActive = value;
                OnPropertyChanged("IsActive"); 
            }
        }

        private List<UseCond> uses = new List<UseCond>(); 
        public  List<UseCond> Uses
        {
            get
            {
                return uses; 
            }
            set
            {
                if (uses != null)
                {
                    foreach (UseCond c in this.Uses)
                    {
                        c.PropertyChanged -= c_PropertyChanged;
                    }
                }
                uses = value;
                if (uses != null)
                {
                    foreach (UseCond c in this.Uses)
                    {
                        c.PropertyChanged += c_PropertyChanged;
                    }
                }
                OnPropertyChanged("Uses"); 
            }
        }
        
        public void Production(UserRec rec)
        {
            lock (Uses)
            {
                if (IsActive)
                {
                    List<RecDemand> Recdem = new List<RecDemand>();
                    foreach (UseCond con in Uses)
                    {
                        Recdem.Add(con.AsRecDem());
                    }

                    if (rec.Use(Recdem))
                    {
                        rec.Increase(ProductionType.Resource, ProductionType.Quantity);
                    }
                }
            }
        }

        public BuildingContainor(string Type, string Id, List<UseCond> Uses, UseCond creates, UseCond creationBonus = null)
        {
            this.Type = Type;
            this.Id = Id;
            this.Uses = Uses;
            ProductionType = creates;
            CreationBonus = creationBonus;
        }

        void creates_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("ProductionType");
        }

        void c_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OnPropertyChanged("Uses");
        }
        
        public void Update(ISharedData data)
        {
            BuildingContainor con = data as BuildingContainor; 
            if(con != null)
            {
                lock (Uses)
                {
                    IsActive = con.IsActive;
                    Uses = uses;
                    ProductionType.Quantity = con.ProductionType.Quantity;
                    ProductionType.Resource = con.ProductionType.Resource; 
                }
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        [Serializable]
        public class UseCond: INotifyPropertyChanged
        {
            string resource;
            public string Resource
            {
                get
                {
                    return resource; 
                }
                set
                {
                    resource = value;
                    OnPropertyChanged("Resource");
                }
            }
 
            public RecDemand AsRecDem()
            {
                return new RecDemand()
                {
                    Rec = Resource,
                    Quantity = Quantity
                };
            }

            int quantity;
            public int Quantity
            {
                get
                {
                    return quantity; 
                }
                set
                {
                    quantity = value;
                    OnPropertyChanged("Quantity");
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
}
