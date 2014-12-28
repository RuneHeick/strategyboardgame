using Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Signals.Building
{

    [Serializable]
    public class MoveSignal:SignalBase
    {
        public string Building { get; set; }

    }
}
