using Logic;
using SharedData;
using SharedData.Types;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLogic
{
    public class UserProduction
    {
        public ObservableCollection<BuildingContainor> Factories { get; private set; }
        DataManager manager;
        public UserRec rec { get; private set;  }

        public UserProduction(DataManager Manager, UserRec rec)
        {
            Factories = new ObservableCollection<BuildingContainor>();
            manager = Manager;
            this.rec = rec; 
            Init();
            manager.CollectionChanged += data_CollectionChanged;
        }

        private void data_CollectionChanged(string arg1, ISharedData item, ChangeType arg3, DataManager manager)
        {
            BuildingContainor b = item as BuildingContainor; 
            if(b != null)
            {
                lock (Factories)
                {
                    Factories.Add(b);
                }
            }
        }

        private void Init()
        {
            string[] names = manager.ItemNames;
            foreach (string name in names)
            {
                if (name.StartsWith("Fac"))
                {
                    BuildingContainor item = manager.GetItem<BuildingContainor>(name);
                    if (item != null)
                    {
                        lock (Factories)
                        {
                            Factories.Add(item);
                        }
                    }
                }
            }
        }

        public void AddFactory(string Type, string Id, List<SharedData.Types.BuildingContainor.UseCond> Uses, SharedData.Types.BuildingContainor.UseCond creates, SharedData.Types.BuildingContainor.UseCond Creationbonus )
        {
            lock (Factories)
            {
                var building = new BuildingContainor(Type, Id, Uses, creates, Creationbonus);
                manager.Add(building);
            }
        }

        public void RemoveFactory(string Id)
        {
            lock (Factories)
            {
                var building = Factories.FirstOrDefault((o)=>o.Id == Id); 
                if(building != null)
                {
                    Factories.Remove(building);
                    manager.Remove(building); 
                }
            }
        }

        public void DoProduction()
        {
            lock(Factories)
            {
                foreach(BuildingContainor b in Factories)
                {
                    b.Production(rec);
                }
            }
        }

        public void GiveBonus(BuildingContainor item)
        {
            if (item.CreationBonus != null)
                rec.Increase(item.CreationBonus.Resource, item.CreationBonus.Quantity);
        }

        public void RemoveBonus(BuildingContainor item)
        {
            if (item.CreationBonus != null)
                rec.Increase(item.CreationBonus.Resource, -item.CreationBonus.Quantity);
        }

        public void AddFactory(BuildingContainor building)
        {
            lock (Factories)
            {
                manager.Add(building);
            }
        }
    }
}
