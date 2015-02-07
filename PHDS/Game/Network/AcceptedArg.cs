using Network.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Network
{
    public class AcceptedArg
    {
        public ClientData Client { get; set; }

        public bool IsAccepted { get; set; }

        public bool Create { get; set; }

        public AcceptedArg(ClientData client)
        {
            IsAccepted = false; 
            Client = client;
        }

    }
}
