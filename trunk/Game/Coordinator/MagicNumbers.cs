using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coordinator
{
    public static class MagicNumbers
    {

        public static int WARWIN_GAMEPOINT = 5;
        public static int ATTACKDEFEND_STARTVALUE = 100;

        public static int ARMYCREATETIME_SECONDS = 10;
        public static int RESEARCH_UPDATETIME_SECONDS = 5; 
        public static int PRODUCTIONCYCLE_SECONDS = 5;

        public static int RESEARCH_PRODUCTIONDEVISION = 5;
        public static int RESEARCH_DEFENTATTACKBONUS = 5;

        public static TimeSpan GAMEPOINT_SAMPLETIME = new TimeSpan(0, 0, 10); 

    }
}
