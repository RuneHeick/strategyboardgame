using Logic;
using SharedData.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coordinator.Logic.Building
{
    [Serializable]
    public class BuildingInfo
    {
        public string Type { get; set; }

        public BuildingContainor.UseCond Produses { get; set; }

        public BuildingContainor.UseCond Bonus { get; set; }

        public List<BuildingContainor.UseCond> Uses { get; set; }

        public List<RecDemand> Cost { get; set; }

        
        public void Load(string path)
        {
            string[] data = File.ReadAllLines(new FileInfo(path).FullName);
            Uses = new List<BuildingContainor.UseCond>();
            Cost = new List<RecDemand>();

            Type = Path.GetFileNameWithoutExtension(path);
            string Mode = ""; 

            for(int i = 0; i <data.Length; i++)
            {
                string[] items = data[i].Split('=');
                if (items.Length == 1)
                    Mode = items[0]; 
                else if(items.Length>1)
                {
                    switch(Mode.Trim())
                    {
                        case "Create":
                            Produses = new BuildingContainor.UseCond() { Resource = items[0], Quantity = Convert.ToInt32(items[1]) };
                            break;

                        case "Give":
                            Bonus = new BuildingContainor.UseCond() { Resource = items[0], Quantity = Convert.ToInt32(items[1]) };
                            break;

                        case "Cost":
                            Uses.Add(new BuildingContainor.UseCond() { Resource = items[0], Quantity = Convert.ToInt32(items[1]) });
                            break;

                        case "BuildingFee":
                            Cost.Add(new RecDemand() { Rec = items[0], Quantity = Convert.ToInt32(items[1]) });
                            break;
                    }
                }
            }


        }

    }
}
