using Logic;
using SharedData;
using SharedData.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace Coordinator.Logic
{
    public class ArmyLogic:ILogic
    {

        ObservableCollection<ContainorItem> Armys = new ObservableCollection<ContainorItem>();
        DispatcherTimer timer = new DispatcherTimer(); 

        public ArmyLogic()
        {
            timer.Tick += timer_Tick;
            timer.Interval = new TimeSpan(0, 0, 10);
            timer.Start(); 
        }

        void timer_Tick(object sender, EventArgs e)
        {
            lock(Armys)
            {
                foreach(ContainorItem army in Armys)
                {
                    for (int i = 0; i < army.BaseCount;i++ )
                    { 
                        if (army.Soldier.Value < army.MaxSize.Value && army.Workers.Value > 0)
                        {
                            army.Soldier.Value++;
                            army.Workers.Value--;
                        }
                    }
                        if (army.Workers.Value < 0 && army.Soldier.Value > 0)
                        {
                            army.Workers.Value++;
                            army.Soldier.Value--;
                        }
                }
            }
        }


        public void Create(string Name, DataManager data)
        {
            lock (Armys)
            {
                Armys.Add(new ContainorItem(data));
            }
        }







        class ContainorItem
        {

            private DataManager manager; 
         
            public ContainorItem(DataManager item)
            {
                manager = item; 
                MaxSize = item.GetItem<IntContainor>("ArmySize");
                if (MaxSize == null)
                {
                    MaxSize = new IntContainor("ArmySize");
                    item.Add(MaxSize);
                }

                string[] names = item.ItemNames;
                foreach(string s in names)
                {
                    if(s.Contains("Soldier"))
                        Soldier = item.GetItem<TagIntContainor>(s);
                    else if(s.Contains("Worker"))
                        Workers = item.GetItem<TagIntContainor>(s);
                    else if(s.Contains("Attack"))
                        Attack = item.GetItem<TagIntContainor>(s);
                    else if(s.Contains("Defence"))
                        Defence = item.GetItem<TagIntContainor>(s);
                }

            }
   
            public IntContainor MaxSize { get; private set;  }

            public TagIntContainor Soldier { get; private set; }

            public TagIntContainor Workers { get; private set; }

            public TagIntContainor Attack { get; private set; }

            public TagIntContainor Defence { get; private set; }

            public int BaseCount
            {
                get
                {
                    int i = 0; 

                    foreach(string s in manager.ItemNames)
                    {

                        if (s.ToLower().Contains("army"))
                        {
                            BuildingContainor item = manager.GetItem<BuildingContainor>(s);
                            if (item != null && item.IsActive)
                            {
                                i++;
                            }
                        }
                    }

                    return i; 
                }
            }
        }

    }




}
