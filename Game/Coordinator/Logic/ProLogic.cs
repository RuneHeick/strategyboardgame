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
using System.Windows.Threading;
using System.Windows;
using SharedLogic.Production;

namespace Coordinator.Logic
{
    class ProLogic:ILogic
    {
        Dictionary<string, BuildingContainor> AllBuildings = new Dictionary<string, BuildingContainor>();
        public Building.BuildingLoader BuildingManager { get; private set;  }
        ObservableCollection<UserProduction> UserBuildings;
        RCLogic recManager;

        DispatcherTimer ProductionTimer;

        public TimeSpan UpdateInterval
        {
            get
            {
                return ProductionTimer.Interval; 
            }
            set
            {
                ProductionTimer.Interval = value; 
            }
        }

        public ProLogic(RCLogic RecManager)
        {
            BuildingManager = new Building.BuildingLoader("Buildings"); 
            UserBuildings = new ObservableCollection<UserProduction>();
            recManager = RecManager;

            ProductionTimer = new DispatcherTimer();
            ProductionTimer.Tick += new EventHandler(timer_Tick);
            ProductionTimer.Interval = new TimeSpan(0, 0, 0, 5, 0);
            ProductionTimer.Start(); 
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lock(UserBuildings)
            {
                foreach(UserProduction up in UserBuildings)
                {
                    up.DoProduction();
                }
            }
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
            data.Add(new BuildingInfoContainor("Buildings"));
            MoveRq.Value = ""; 

            MoveRq.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler((o,p)=> MoveRqChanged(o,p,pro));

            data.CollectionChanged += new Action<string,ISharedData,ChangeType,DataManager>((a,b,c,d)=>data_CollectionChanged(a,b,c,d,pro));
        }

        void data_CollectionChanged(string arg1, ISharedData arg2, ChangeType arg3, DataManager arg4, UserProduction pro)
        {
            if (arg3 == ChangeType.Added)
            {
                CreateBuildingRq Rq = arg2 as CreateBuildingRq;
                if (Rq != null)
                {
                    AddBuilding(Rq, arg4, pro);
                    arg4.Remove(Rq);
                }
            }
        }

        private void AddBuilding(CreateBuildingRq Rq, DataManager data, UserProduction pro)
        {
            var b = BuildingManager.GetBuilding(Rq.Type, pro.rec);
            if(b != null)
            {
                data.Add(b);
            }
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
                item.PropertyChanged += (s,p) => OnActiveChanged(s,p,pro); 
            }
            else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var item = e.OldItems[0] as BuildingContainor;
                pro.RemoveBonus(item);
                item.PropertyChanged -= (s,p) => OnActiveChanged(s,p,pro);
            }
        }

        private void OnActiveChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e, UserProduction pro )
        {
            var item = sender as BuildingContainor;
            if(e.PropertyName == "IsActive" )
            {
                if(item.IsActive)
                {
                    pro.GiveBonus(item);
                }
                else
                {
                    pro.RemoveBonus(item);
                }
            }
        }


  
    }
}
