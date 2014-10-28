using Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using SharedData.Types;
using SharedLogic.Production;
using SharedData;
using System.Windows.Threading; 

namespace Coordinator.Logic
{
    public class ResearchLogic: ILogic
    {
        public static string[] ResearchItems = new string[] { "Attack", "Defence", "Knowlage", "Buildings", "Production" };


        DispatcherTimer timer = new DispatcherTimer(); 
        private ObservableCollection<ResearchProfil> Profiles = new ObservableCollection<ResearchProfil>();

        private Dictionary<string, List<Action<string, DataManager>>> handles = new Dictionary<string, List<Action<string, DataManager>>>(); 
        
        public ResearchLogic()
        {
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Tick += timer_Tick;
            timer.Start(); 
        }

        void timer_Tick(object sender, EventArgs e)
        {
            lock (Profiles)
            {
                foreach(ResearchProfil p in Profiles)
                {
                    DoResearch(p); 
                }
            }
        }

        public void Create(string Name, DataManager data)
        {
            lock (Profiles)
                Profiles.Add(new ResearchProfil(data));
        }

        public void AddUpdate(Action<string, DataManager> handle, string item)
        {
            lock (handles)
            {
                if(handles.ContainsKey(item))
                {
                    handles[item].Add(handle); 
                }
                else
                {
                    var list = new List<Action<string, DataManager>>(); 
                    list.Add(handle);
                    handles.Add(item, list); 
                }
            }
        }

        public void RemoveItem(Action<string, DataManager> handle)
        {
            lock (handles)
            {
                foreach (List<Action<string, DataManager>> list in handles.Values)
                {
                    if (list.Contains(handle))
                    {
                        list.Remove(handle);
                        return;
                    }
                }
            }
        }

        private void DoResearch(ResearchProfil profile)
        {
            lock (profile.Schools)
            {
                var reBonus = profile.ResearchValues.FirstOrDefault((o) => o.RealName == "Knowlage");

                double bonus = ((int)reBonus.Value) / 10.0;

                foreach (SchoolContainor school in profile.Schools)
                {
                    if (school.IsActive)
                    {
                        var item = school.DoResearch();
                        var reitem = profile.ResearchValues.FirstOrDefault((o) => o.RealName == item.Name);
                        var oldret = profile.ResearchValuesTemps.FirstOrDefault((o) => o.RealName == item.Name);

                        if (reitem != null)
                        {
                            reitem.Value += 0.25 + bonus;


                            if (((int)reitem.Value) > ((int)oldret.Value))
                            {
                                if (handles.ContainsKey(reitem.RealName))
                                {
                                    for (int i = ((int)oldret.Value); i < ((int)reitem.Value); i++)
                                    {
                                        var handleslist = handles[reitem.RealName];
                                        foreach (Action<string, DataManager> handle in handleslist)
                                        {
                                            handle(reitem.RealName, profile.Manager);
                                        }
                                    }
                                }
                                oldret.Value = reitem.Value;
                            }
                        }


                    }
                }
            }

        }

        public DoubleTagContainor GetResearchStats(string name, DataManager manager)
        {
            ResearchProfil profil = Profiles.FirstOrDefault((o) => o.Manager == manager);
            if(profil != null)
            {
                var item = profil.ResearchValues.FirstOrDefault((o) => o.RealName == name);
                return item; 
            }

            return null; 
        }

        private class ResearchProfil
        {

            public DataManager Manager { get; set; }

            public ResearchProfil(DataManager manager)
            {
                Manager = manager; 

                ResearchValues = new ObservableCollection<DoubleTagContainor>();
                ResearchValuesTemps = new ObservableCollection<DoubleTagContainor>(); 
                Schools = new ObservableCollection<SchoolContainor>();

                Init(ResearchValues);

                manager.CollectionChanged += manager_CollectionChanged; 
            }

            void manager_CollectionChanged(string arg1, ISharedData arg2, ChangeType arg3, DataManager arg4)
            {
                var school = arg2 as SchoolContainor;
                if (school != null)
                {
                    lock (Schools)
                    {
                        if (arg3 == ChangeType.Added)
                        {
                            Schools.Add(school);
                        }
                        else
                        {
                            Schools.Remove(school);
                        }
                    }
                }
            }

            private void Init(ObservableCollection<DoubleTagContainor> ResearchValues)
            {
                foreach (string s in ResearchItems)
                {
                    var item = new DoubleTagContainor(s,"Research");
                    ResearchValues.Add(item);
                    Manager.Add(item);

                    ResearchValuesTemps.Add(new DoubleTagContainor(s, "Research"));

                }

            }

            public ObservableCollection<DoubleTagContainor> ResearchValues { get; set; }
            public ObservableCollection<DoubleTagContainor> ResearchValuesTemps { get; set; }

            public ObservableCollection<SchoolContainor> Schools { get; set; }

        }

    }
}
