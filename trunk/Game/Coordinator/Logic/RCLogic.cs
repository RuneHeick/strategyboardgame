using SharedData;
using SharedData.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using Logic;

namespace Coordinator.Logic
{
    public class RCLogic : ILogic
    {
        public List<UserRec> managers = new List<UserRec>();
        private ResearchLogic researchManager;

        public RCLogic(ResearchLogic ResearchManager)
        {
            // TODO: Complete member initialization
            this.researchManager = ResearchManager;

            researchManager.AddUpdate(ResearchComplete, "Attack");
            researchManager.AddUpdate(ResearchComplete, "Defence");
        }

        private void ResearchComplete(string name, DataManager manager )
        {
            UserRec rec = managers.FirstOrDefault((o)=> o.manager == manager);
            if(rec != null)
            {
                rec.Increase(name, 5);
            }
        }

        public void Create(string name, DataManager data)
        {
            var user = new UserRec(data);
            managers.Add(user);
            
            user.AddRec("Water",50);
            user.AddRec("Food", 50);
            user.AddRec("Power", 50);
            user.AddRec("Game Point", 50);
            user.AddRec("Attack", 100);
            user.AddRec("Defence", 100);
            user.AddRec("Soldier", 0);
            user.AddRec("Worker", 0);
        }

    }


}

