﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coordinator.ViewModel.SelectedViewModel
{
    public class Resources : SelectedBase
    {

        public string Image { get; set; }

        public Resources()
        {
            Image = "Worker";
        }

        public override void DoAction(SharedData.DataManager manager, MainWindowViewModel mainVindow)
        {
            mainVindow.ProManager.DoProduction(10, manager); 
        }
        

    }
}
