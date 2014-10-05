using SharedData;
using SharedData.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using Logic;
using SharedLogic;

namespace Coordinator.Logic
{
    class ProLogic:ILogic
    {
        ObservableCollection<UserProduction> UserBuildings;
        RCLogic recManager;

        public ProLogic(RCLogic RecManager)
        {
            UserBuildings = new ObservableCollection<UserProduction>();
            recManager = RecManager; 
        }

        public void Create(string name, DataManager data)
        {
            UserRec rec = recManager.managers.FirstOrDefault((o) => o.manager == data); 
            UserProduction pro = new UserProduction(data, rec);
            lock (UserBuildings)
            {
                UserBuildings.Add(pro);
            }

            pro.Factories.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler((sender, e) =>Factories_CollectionChanged(sender,e,pro));
            StringContainor MoveRq = new StringContainor("BuildingMove");
            data.Add(MoveRq);
            MoveRq.Value = ""; 

            MoveRq.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((o,p)=> MoveRqChanged(o,p,pro));
        }

        private void MoveRqChanged(object o, System.ComponentModel.PropertyChangedEventArgs p, UserProduction pro)
        {
            StringContainor MoveRq = o as StringContainor; 
            if(MoveRq != null)
            {
                string building = MoveRq.Value;
                building= building.ToLower(); 
                MoveRq.Value = "";

                lock (UserBuildings)
                {
                    foreach(UserProduction usrpro in UserBuildings)
                    {
                        var item = usrpro.Factories.FirstOrDefault((u) => u.Id.ToLower() == building);
                        if(item != null)
                        {
                            usrpro.RemoveFactory(item.Id);
                            pro.AddFactory(item);
                            return;
                        }
                    }
                }
            }
        }

        void Factories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e, UserProduction pro )
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var item = e.NewItems[0] as BuildingContainor;
                pro.GiveBonus(item);
            }
            else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var item = e.NewItems[0] as BuildingContainor;
                pro.RemoveBonus(item);
            }
        }


    }
}
