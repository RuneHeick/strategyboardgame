using Logic;
using SharedData.Types;
using SharedLogic;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLogic
{
    class ProductionMatrix
    {
        ObservableCollection<ItemContainor> FactoriesStats = new ObservableCollection<ItemContainor>();
        UserProduction Production;


        public ProductionMatrix(UserProduction Pro)
        {
            Production = Pro; 
        }


        public void Produce()
        {
            var sum = DoSum();

            foreach (var rec in Production.rec.Rec)
            {
                sum[rec.RealName] = (-1*rec.Value);
            }

            foreach (string key in sum.Uses.Keys)
            {
                Production.rec.Increase(key, sum[key]);
            }
        }

        private void Create()
        {
            FactoriesStats.Clear();
            foreach (BuildingContainor bulding in Production.Factories)
            {
                if (bulding.IsActive)
                    FactoriesStats.Add(new ItemContainor(bulding));
            }
        }

        private ItemContainor DoSum()
        {
            Create();
            var sum = new ItemContainor(); 

            foreach(var rec in Production.rec.Rec)
            {
                sum[rec.RealName] = rec.Value;
            }


            foreach(ItemContainor fac in FactoriesStats)
            {
                foreach(string key in fac.Uses.Keys)
                {
                    sum[key] = (-1*fac.Uses[key]);
                }
            }


            foreach (string key in sum.Uses.Keys)
            {
                if (0 > sum[key])
                {
                    CloseFactory(key, sum[key]);
                    return DoSum();
                }
            }
            return sum; 
        }

        private void CloseFactory(string key, int closeSum)
        {
            foreach (var fac in Production.Factories)
            {
                foreach(var rec in fac.Uses)
                {
                    if(rec.Resource == key)
                    {
                        closeSum -= rec.Quantity;
                        if (fac.IsActive)
                        {
                            fac.IsActive = false;
                        }
                        if(closeSum>=0)
                            return; 
                    }

                }
                if(fac.CreationBonus.Resource == key)
                {
                    closeSum -= fac.CreationBonus.Quantity;
                    if (fac.IsActive)
                    {
                        fac.IsActive = false;
                    }
                    if (closeSum >= 0)
                        return; 
                }

            }
        }


        private class ItemContainor
        {
            public ItemContainor()
            {
                 Uses = new Dictionary<string,int>(); 
            }

            public ItemContainor(BuildingContainor item)
            {
                Uses = new Dictionary<string,int>(); 
                FactoryBuilding = item.Id;

                if (item.CreationBonus != null)
                    this[item.CreationBonus.Resource] = item.CreationBonus.Quantity; 
                if(item.ProductionType != null)
                    this[item.ProductionType.Resource] = item.ProductionType.Quantity; 

                foreach(var useItem in item.Uses)
                {
                    this[useItem.Resource] = useItem.Quantity;
                }

            }

            public string FactoryBuilding { get; set; }

            public Dictionary<string, int> Uses {get; set; }

            public int this[string index]
            {
                get
                {
                    if (Uses.ContainsKey(index))
                        return Uses[index];
                    return 0; 
                }
                set
                {
                    if (!Uses.ContainsKey(index))
                        Uses.Add(index, value);
                    else
                        Uses[index] += value; 
                }

            }


        }
    }
}
