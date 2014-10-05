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

        public BuildingInfoContainor(string path)
        {
            Name = "Buildings";
            Buildings = new List<BuildingInfo>();
            BuildingFolder = new DirectoryInfo(path);

            FileInfo[] Files = BuildingFolder.GetFiles();
            foreach (FileInfo f in Files)
            {
                var b = new BuildingInfo();
                b.Load(f.FullName);
                Buildings.Add(b);
            }
        }
        
        public void Update(ISharedData data)
        {
            CreateBuildingRq con = data as CreateBuildingRq; 
            if(con != null)
            {
                
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

