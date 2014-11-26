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
using SharedLogic.Global;

namespace Coordinator.Logic
{
    public class ArmyLogic:ILogic
    {

        ObservableCollection<ContainorItem> Armys = new ObservableCollection<ContainorItem>();
        DispatcherTimer timer = new DispatcherTimer();

        ObservableCollection<WarContaionor> Wars = new ObservableCollection<WarContaionor>();
        Random warSelector = new Random(); 
        int warCount = 0; 


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
                Armys.Add(new ContainorItem(data, Name));
            }
        }


        public void StartWar(DataManager Attacker)
        {
            lock (Wars)
            {
                var item = Armys.FirstOrDefault((o) => o.manager == Attacker);
                if (item != null)
                {
                    warCount++;
                    var war = new WarContaionor(warCount);
                    var warAttacker = new WarContaionor.CollectionItem();


                    warAttacker.Name = item.Name;
                    warAttacker.WarSkill = item.Attack.Value;
                    warAttacker.Alive = 0;

                    war.Attacker = warAttacker; 
                    Wars.Add(war);

                    war.PropertyChanged += Attacker_PropertyChanged;
                    Attacker.Add(war);
                }
            }

        }

        void Attacker_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            WarContaionor war = sender as WarContaionor; 
            if(war != null && war.Attacker != null && war.Defender != null)
            {
                war.PropertyChanged -= Attacker_PropertyChanged;

                var DefenterArmy = Armys.FirstOrDefault((o) => o.Name == war.Defender.Name);
                if(DefenterArmy != null)
                {
                    var warDefenter = new WarContaionor.CollectionItem();
                    
                    warDefenter.WarSkill = DefenterArmy.Defence.Value;
                    warDefenter.Alive = 0;
                    warDefenter.Name = DefenterArmy.Name;

                    war.Defender = warDefenter; 

                    DefenterArmy.manager.Add(war);
                    war.PropertyChanged += Defender_PropertyChanged;
                }
                else
                {
                    war.PropertyChanged += Attacker_PropertyChanged;
                }
            }
            else
            {

            }
        }


        void Defender_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            WarContaionor item = sender as WarContaionor;
            if (item != null && item.Defender.IsDone )
            {
                
                item.PropertyChanged -= Defender_PropertyChanged;
                DoWar(item);
            }
        }

        private void DoWar(WarContaionor war)
        {
            var AttackerArmy = Armys.FirstOrDefault((o) => o.Name == war.Attacker.Name);
            var DefenterArmy = Armys.FirstOrDefault((o) => o.Name == war.Defender.Name);
            if(AttackerArmy != null && DefenterArmy != null)
            {
                AttackerArmy.Soldier.Value -= war.Attacker.Alive;
                DefenterArmy.Soldier.Value -= war.Defender.Alive;

                List<int> AttackerSoldiers = CreateArmy(war.Attacker.Alive);
                List<int> DefenderSoldiers = CreateArmy(war.Defender.Alive);
 
                while(AttackerSoldiers.Count >0 && DefenderSoldiers.Count> 0)
                {
                    int AHit = warSelector.Next(war.Attacker.WarSkill - 100, war.Attacker.WarSkill);
                    int BHit = warSelector.Next(war.Defender.WarSkill - 100, war.Defender.WarSkill);

                    AttackerSoldiers[0] -= BHit;
                    DefenderSoldiers[0] -= AHit;

                    if (AttackerSoldiers[0] < 0)
                        AttackerSoldiers.RemoveAt(0);

                    if (DefenderSoldiers[0] < 0)
                        DefenderSoldiers.RemoveAt(0); 

                }

                AttackerArmy.Soldier.Value += AttackerSoldiers.Count;
                AttackerArmy.Workers.Value += (war.Attacker.Alive - AttackerSoldiers.Count);

                DefenterArmy.Soldier.Value += DefenderSoldiers.Count;
                DefenterArmy.Workers.Value += (war.Defender.Alive - DefenderSoldiers.Count);

                WarResultContaionor result = new WarResultContaionor(war.ID);
                if(AttackerSoldiers.Count == 0)
                {
                    result.Loser = war.Attacker.Name;
                    result.Winner = war.Defender.Name;
                }
                else
                {
                    result.Loser = war.Defender.Name;
                    result.Winner = war.Attacker.Name;
                }

                AttackerArmy.manager.Add(result);
                DefenterArmy.manager.Add(result);

            }

        }

        private List<int> CreateArmy(int i)
        {
            List<int> army = new List<int>(i);
            for (int z = 0; z < i; z++)
                army.Add(1000);
            return army; 
        }




        class ContainorItem
        {

            public DataManager manager;
            public string Name { get; private set; }

            public ContainorItem(DataManager item, string Name)
            {
                this.Name = Name;
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
