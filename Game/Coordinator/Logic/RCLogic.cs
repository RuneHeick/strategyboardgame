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

        public void Create(string name, DataManager data)
        {
            var user = new UserRec(data);
            managers.Add(user);
            
            user.AddRec("Houses",0);
            user.AddRec("Water",50);
            user.AddRec("Food", 50);
            user.AddRec("Power", 50);
            user.AddRec("Game Point", 50);
        }

    }


}

