using Logic;
using Network.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharedLogic.Global;

namespace Coordinator.Logic
{
    public class OnlineManagerLogic:ILogic
    {

        public UsersList Users { get; set; }

        public OnlineManagerLogic()
        {
            Users = new UsersList(); 
        }

        public void Login(ClientData User)
        {
            Users.Increase(User.LoginName);
        }

        public void Disconnect(ClientData User)
        {
            Users.Decrease(User.LoginName);
        }

        public void Create(string Name, SharedData.DataManager data)
        {
            data.Add(Users);
        }
    }
}
