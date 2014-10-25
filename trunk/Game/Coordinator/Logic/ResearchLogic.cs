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
        DispatcherTimer timer = new DispatcherTimer(); 
        private ObservableCollection<ResearchProfil> Profiles = new ObservableCollection<ResearchProfil>(); 

        
        public ResearchLogic()
        {
            timer.Interval = new TimeSpan(0, 0, 15);
            timer.Tick += timer_Tick;
            timer.Start(); 
        }

        void timer_Tick(object sender, EventArgs e)
        {
            lock (Profiles)
            {
                foreach(ResearchProfil p in Profiles)
                {
                    p.DoResearch(); 
                }
            }
        }

        public void Create(string Name, DataManager data)
        {
            lock (Profiles)
                Profiles.Add(new ResearchProfil(data)); 
        }


        private class ResearchProfil
        {

            static string[] data = new string[] { "Attack", "Defence", "Knowlage", "Military", "Buildings", "Production" };

            public DataManager Manager { get; set; }

            public ResearchProfil(DataManager manager)
            {
                Manager = manager; 

                ResearchValues = new ObservableCollection<DoubleTagContainor>();
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
                foreach(string s in data)
                {
                    var item = new DoubleTagContainor(s,"Research");
                    ResearchValues.Add(item);
                    Manager.Add(item); 
                }

            }

            public ObservableCollection<DoubleTagContainor> ResearchValues { get; set; }

            public ObservableCollection<SchoolContainor> Schools { get; set; }

            public void DoResearch()
            {
                lock(Schools)
                {
                    var reBonus = ResearchValues.FirstOrDefault((o) => o.RealName == "Knowlage");

                    double bonus = ((int)reBonus.Value)/10.0;

                    foreach(SchoolContainor school in Schools)
                    {
                        if (school.IsActive)
                        {
                            var item = school.DoResearch();
                            var reitem = ResearchValues.FirstOrDefault((o) => o.RealName == item.Name);
                            if (reitem != null)
                                reitem.Value += 0.25 + bonus;
                        }
                    }
                }
            }

        }

    }
}
