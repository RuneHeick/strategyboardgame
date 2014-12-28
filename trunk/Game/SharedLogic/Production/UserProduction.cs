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
        private ProductionMatrix productionMatrix; 


        public UserProduction(DataManager Manager, UserRec rec)
        {
            productionMatrix = new ProductionMatrix(this);
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
                    if (arg3 == ChangeType.Added)
                        Factories.Add(b);
                    else if (arg3 == ChangeType.Removed)
                        Factories.Remove(b);
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

        public void RemoveFactory(string Id)
        {
            lock (Factories)
            {
                var building = Factories.FirstOrDefault((o)=>o.Id == Id); 
                if(building != null)
                {
                    manager.Remove(building); 
                }
            }
        }

        public void DoProduction(int Bonus)
        {
            lock(Factories)
            {
                productionMatrix.Produce(); 
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
