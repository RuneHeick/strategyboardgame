using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using SharedData.Types;
using Logic;

namespace Coordinator.Logic.Building
{
    public class BuildingLoader
    {
        DirectoryInfo BuildingFolder;
 
        List<string> RandomList = new List<string>(); 
        int UsedRandom = 0; 

        public List<BuildingInfo> Buildings {get; set;}

        public BuildingLoader(string path)
        {
            Buildings = new List<BuildingInfo>(); 
            BuildingFolder = new DirectoryInfo(path); 

            FileInfo[] Files = BuildingFolder.GetFiles(); 
            foreach(FileInfo f in Files)
            {
                var b = new BuildingInfo(); 
                b.Load(f.FullName); 
                Buildings.Add(b); 
            }

        }

        private void InitRandom()
        {
            int count = RandomList.Count; 

            while(RandomList.Count < count+20)
            {
                string value = NextRandom();

 	            if(!RandomList.Contains(value))
                {
                    RandomList.Add(value); 
                }
            }
        }

        public string GetRandom()
        {
            if(UsedRandom == RandomList.Count)
                InitRandom();
            string ret = RandomList[UsedRandom];
            UsedRandom++; 
            return ret; 
        }

        private string NextRandom()
        {
            var chars = "ABCDEFGHJKLMNOPQRSTUVWXYZ123456789";
            var random = new Random();
            var result = new string(
            Enumerable.Repeat(chars, 6)
              .Select(s => s[random.Next(s.Length)])
              .ToArray());

            return result; 
        }

        public BuildingContainor GetBuilding(string Name, UserRec Users)
        {
            BuildingInfo item = Buildings.FirstOrDefault((o)=>o.Type == Name); 
            if(item != null)
            {
                if(Users.Use(item.Cost))
                {
                    BuildingContainor building = new BuildingContainor(item.Type,GetRandom(),item.Uses,item.Produses,item.Bonus );
                    building.IsActive = true; 
                    return building; 
                }
            }

            return null; 
        }




        
        }


    }
