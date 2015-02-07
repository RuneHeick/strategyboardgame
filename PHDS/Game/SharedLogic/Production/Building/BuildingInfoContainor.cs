using Coordinator.Logic.Building;
using Logic;
using SharedData;
using SharedData.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLogic.Production
{
    [Serializable]
    public class BuildingInfoContainor:ISharedData
    {
        public string Name { get; set; }

        public List<BuildingInfo> Buildings {get; private set;}


        public BuildingInfoContainor(string path, int level = 0)
        {
            Name = "Buildings";
            Buildings = new List<BuildingInfo>();
            BuildingFolder = new DirectoryInfo(path);
            SetLevel(level); 
        }

        public void SetLevel(int level)
        {
            Buildings.Clear(); 
            FileInfo[] Files = BuildingFolder.GetFiles();
            foreach (FileInfo f in Files)
            {
                var b = new BuildingInfo();
                b.Load(f.FullName);
                if(b.Level <= level)
                    Buildings.Add(b);
            }
            OnPropertyChanged("Buildings");
        }
        
        public void Update(ISharedData data)
        {
            BuildingInfoContainor con = data as BuildingInfoContainor; 
            if(con != null)
            {
                Buildings = con.Buildings;
                OnPropertyChanged("Buildings");
            }
        }

        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }


        public DirectoryInfo BuildingFolder { get; set; }
    }
}

