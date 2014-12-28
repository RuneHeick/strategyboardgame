using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals.War
{
    [Serializable]
    public class WarResultSignal: SignalBase
    {

        public string Winner { get; set; }

        public string Loser { get; set; }

    }
}
