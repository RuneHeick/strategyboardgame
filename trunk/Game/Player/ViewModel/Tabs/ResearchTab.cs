using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;
using SharedLogic.Production;
using SharedData;

namespace Player.ViewModel.Tabs
{
    public class ResearchTab:TabBase
    {

        

        public ObservableCollection<ResearchItem> ResearchItems { get; set; }
        public ObservableCollection<SchoolContainor> Schools { get; set; }
        


        public ResearchTab()
        {
            Name = "Research";
            ResearchItems = new ObservableCollection<ResearchItem>();

            ResearchItems.Add(new ResearchItem("Attack", ""));
            ResearchItems.Add(new ResearchItem("Defence", ""));
            ResearchItems.Add(new ResearchItem("Production", ""));
            ResearchItems.Add(new ResearchItem("Knowlage", ""));

            Schools = new ObservableCollection<SchoolContainor>();
            PlayerData.Instance.Client.dataManager.CollectionChanged += dataManager_CollectionChanged;

            Init(PlayerData.Instance.Client.dataManager);

           
        }

        private void Init(SharedData.DataManager dataManager)
        {
            Schools.Clear();
            string[] names = dataManager.ItemNames;
            foreach(string name in names)
            {
                if(name.ToLower().Contains("school"))
                {
                    SchoolContainor school = dataManager.GetItem<SchoolContainor>(name);
                    if (school != null)
                        dataManager_CollectionChanged(null, school, ChangeType.Added, dataManager); 
                }                
            }


        }

        void dataManager_CollectionChanged(string arg1, ISharedData arg2, ChangeType arg3, DataManager arg4)
        {
            var school = arg2 as SchoolContainor;
            if (school != null)
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
}
