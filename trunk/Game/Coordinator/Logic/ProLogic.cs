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
using Coordinator.Logic.Building;

namespace Coordinator.Logic
{
    public class ProLogic:ILogic
    {
        Dictionary<string, BuildingContainor> AllBuildings = new Dictionary<string, BuildingContainor>();
        public Building.BuildingLoader BuildingManager { get; private set;  }
        ObservableCollection<UserProduction> UserBuildings;
        RCLogic recManager;

        private const int RESEARCHDIVISION = 5;

        DispatcherTimer ProductionTimer;
        private ResearchLogic ResearchManager;
        public int MaxBuildingLevel { get; set; }


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



        public ProLogic(RCLogic RecManager, ResearchLogic ResearchManager)
        {
            BuildingManager = new Building.BuildingLoader("Buildings");
            UserBuildings = new ObservableCollection<UserProduction>();
            recManager = RecManager;

            ProductionTimer = new DispatcherTimer();
            ProductionTimer.Tick += new EventHandler(timer_Tick);
            ProductionTimer.Interval = new TimeSpan(0, 0, 0, 5, 0);
            ProductionTimer.Start(); 
            this.ResearchManager = ResearchManager;
            ResearchManager.AddUpdate(BuildingsLevelChanged, "Buildings");

            // find max level 
            var dummy = new BuildingInfoContainor("Buildings", int.MaxValue);
            MaxBuildingLevel = 0; 
            foreach(BuildingInfo b in dummy.Buildings)
            {
                if (b.Level > MaxBuildingLevel)
                    MaxBuildingLevel = (int)b.Level; 
            }

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            lock(UserBuildings)
            {
                foreach(UserProduction up in UserBuildings)
                {
                    var Bonus = ResearchManager.GetResearchStats("Production", up.rec.manager);
                    up.DoProduction(Bonus != null ? (int)Bonus.Value / RESEARCHDIVISION : 0);
                }
            }
        }


        public void DoProduction(int cycles, DataManager manager)
        {
            UserProduction up = UserBuildings.FirstOrDefault((o) => o.rec.manager == manager);
            if (up != null)
            {
                var Bonus = ResearchManager.GetResearchStats("Production", up.rec.manager);
                for(int i = 0; i<cycles; i++)
                {
                    up.DoProduction(Bonus != null ? (int)Bonus.Value / RESEARCHDIVISION : 0);
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


            var levelContaionor = ResearchManager.GetResearchStats("Buildings", data);
            int level = 0;
            if (levelContaionor != null)
                level = (int)levelContaionor.Value; 
            data.Add(new BuildingInfoContainor("Buildings",level));
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
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => ClaimBuilding(MoveRq, pro)));
            }
        }

        void ClaimBuilding(StringContainor MoveRq, UserProduction pro)
        {
            if (MoveRq.Value == "") return;
            string building = MoveRq.Value;
            building = building.ToLower();
            MoveRq.Value = "";

            lock (UserBuildings)
            {
                foreach (UserProduction usrpro in UserBuildings)
                {
                    var item = usrpro.Factories.FirstOrDefault((u) => u.Id.ToLower() == building);
                    if (item != null)
                    {
                        usrpro.RemoveFactory(item.Id);
                        pro.AddFactory(item);
                        return;
                    }
                }
            }
        }

        void Factories_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e, UserProduction pro )
        {
            if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                var item = e.NewItems[0] as BuildingContainor;
                if(item.IsActive)
                    pro.GiveBonus(item);
                item.PropertyChanged +=  OnActiveChanged; 
            }
            else if(e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                var item = e.OldItems[0] as BuildingContainor;
                if (item.IsActive)
                    pro.RemoveBonus(item);
                item.PropertyChanged -=  OnActiveChanged;
            }
        }

        private void OnActiveChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var item = sender as BuildingContainor;
            
            if(e.PropertyName == "IsActive" )
            {
                UserProduction pro = UserBuildings.FirstOrDefault((o) => o.Factories.Contains(item));
                if (pro == null) return; 
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


        private void BuildingsLevelChanged(string name, DataManager manager)
        {
            var levelContaionor = ResearchManager.GetResearchStats(name, manager);

            if (levelContaionor != null)
            {
                if (levelContaionor.Value >= MaxBuildingLevel)
                {
                    manager.Remove(levelContaionor);
                }

                var item = manager.GetItem<BuildingInfoContainor>("Buildings");
                if (item != null)
                    item.SetLevel((int)levelContaionor.Value); 

            }


        }
  
    }
}
