using SharedData.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using Utility.ViewModel;

namespace Player.ViewModel
{
    public class NewBuildingViewModel:ViewModelBase
    {
        public NewBuildingViewModel(BuildingContainor contaionr)
        {
            Id = contaionr.Id;
            Type = contaionr.Type; 
        }

        public string Id { get; set;  }

        public string Type { get; set; }

        public Brush Color
        {
            get
            {
                switch(Type.ToLower())
                {
                    case "farm":
                        return Brushes.Yellow;
                    case "water plant":
                        return Brushes.Blue;
                    case "power plant":
                        return Brushes.Red;
                    case "house":
                        return Brushes.Green;
                    default:
                        return Brushes.Gray;
                }
                
            }
        }

        private RelayCommand closeCommand;
        public ICommand DoneCommand
        {
            get
            {
                if (closeCommand == null)
                    closeCommand = new RelayCommand((p) => closeCommandExecute());
                return closeCommand;
            }
        }

        private void closeCommandExecute()
        {
            CreateCloseRequest(); 
        }

    }
}
