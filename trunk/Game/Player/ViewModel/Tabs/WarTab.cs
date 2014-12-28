using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedData.Types;
using System.Collections.ObjectModel;
using SharedLogic.Global;
using SharedData;
using Signals.War;

namespace Player.ViewModel.Tabs
{
    public class WarTab: TabBase
    {
        TagIntContainor defence; 
        public TagIntContainor Defence
        {
            get
            {
                return defence;
            }
            set
            {
                defence = value;
                OnPropertyChanged("Defence");
            }
        }

        TagIntContainor attack; 
        public TagIntContainor Attack
        {
            get
            {
                return attack;
            }
            set
            {
                attack = value;
                OnPropertyChanged("Attack");
            }
        }


        TagIntContainor soldier;
        public TagIntContainor Soldier
        {
            get
            {
                return soldier;
            }
            set
            {
                soldier = value;
                OnPropertyChanged("Soilders");
            }
        }


        TagIntContainor workers;
        public TagIntContainor Workers
        {
            get
            {
                return workers;
            }
            set
            {
                workers = value;
                OnPropertyChanged("Workers");
            }
        }

        IntContainor armySize = null;
        public int ArmySize
        {
            get
            {
                if(armySize == null)
                {
                    armySize = PlayerData.Instance.Client.dataManager.GetItem<IntContainor>("ArmySize");
                    if(armySize == null)
                    {
                        armySize = new IntContainor("ArmySize");
                        PlayerData.Instance.Client.dataManager.Add(armySize);
                    }
                }

                return armySize.Value; 
            }
            set
            {
                if (armySize != null)
                    armySize.Value = value; 
            }
        }

        private ObservableCollection<BuildingContainor> buildings
        {
            get;
            set; 
        }

        public ObservableCollection<BuildingContainor> ArmyBases { get; private set; }

        public WarTab(MyBuildingsTab mybuildings)
        {
            Visible = false; 
            buildings = mybuildings.Buildings; 
            Name = "Army";
            ArmyBases = new ObservableCollection<BuildingContainor>();
            buildings.CollectionChanged += buildings_CollectionChanged;
            Init();
            PlayerData.Instance.Client.dataManager.CollectionChanged += dataManager_CollectionChanged;
            buildings_CollectionChanged(null, null); 
        }

        void buildings_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            ArmyBases.Clear(); 
            foreach(BuildingContainor b in buildings)
            {
                if (b.Type.ToLower().Contains("army"))
                    ArmyBases.Add(b); 
            }

            Visible = ArmyBases.Count > 0 ? true : false; 
        }

        private void Init()
        {
            string[] names = PlayerData.Instance.Manager.ItemNames;
            foreach (string name in names)
            {
                if (name.Contains("Resources") || name.Contains("War"))
                {
                    ISharedData item = PlayerData.Instance.Client.dataManager.GetItem<ISharedData>(name);
                    if (item != null)
                    {
                        dataManager_CollectionChanged(item.Name, item, SharedData.ChangeType.Added, null);
                    }
                }
            }

            PlayerData.Instance.Manager.Signal.AddSignalHandler<WarAttackRqSignal>(StartWar);
            PlayerData.Instance.Manager.Signal.AddSignalHandler<WarDefenceRqSignal>(EndWar);
            PlayerData.Instance.Manager.Signal.AddSignalHandler<WarResultSignal>(WarResult);
        }

        private void WarResult(WarResultSignal res)
        {
            if (res.Winner == PlayerData.Instance.Client.LoginName)
            {
                PlayerData.Instance.SwitchView(new WinViewModel(res), ViewPrioity.Top);
            }
            else
            {
                PlayerData.Instance.SwitchView(new LoseViewModel(res), ViewPrioity.Top);
            }
        }

        private void StartWar(WarAttackRqSignal obj)
        {
            obj.Done = false;
            PlayerData.Instance.SwitchView(new WarViewModel(obj, Soldier.Value), ViewPrioity.Top);
        }

        private void EndWar(WarDefenceRqSignal obj)
        {
            obj.Done = false;
            PlayerData.Instance.SwitchView(new WarViewModel(obj, Soldier.Value), ViewPrioity.Top);
        }

 
        void dataManager_CollectionChanged(string arg1, SharedData.ISharedData arg2, SharedData.ChangeType arg3, SharedData.DataManager arg4)
        {
            TagIntContainor item = arg2 as TagIntContainor; 
            if(item != null)
            {
                if(item.RealName=="Attack")
                {
                    Attack = item; 
                }
                else if(item.RealName=="Defence")
                {
                    Defence = item; 
                }
                else if (item.RealName == "Worker")
                {
                    Workers = item;
                }
                else if (item.RealName == "Soldier")
                {
                    Soldier = item;
                }
            }

        }



    }
}
