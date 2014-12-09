using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coordinator.ViewModel.SelectedViewModel
{
    public class Research : SelectedBase
    {

        public string SelectedName { get; set; }


        public override void DoAction(SharedData.DataManager manager, MainWindowViewModel mainVindow)
        {
            var item = mainVindow.ResearchManager.Profiles.FirstOrDefault((o) => o.Manager == manager);
            if (item != null)
            {
                Random ran = new Random(DateTime.Now.Millisecond);
                int index = ran.Next(0, item.ResearchValues.Count);

                var recItem = item.ResearchValues[index];
                SelectedName = recItem.RealName;

                mainVindow.ResearchManager.IncreaseValue(1, 0, recItem.RealName, manager);
            }
        }

    }
}
