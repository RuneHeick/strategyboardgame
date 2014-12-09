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
                string type = Type.ToLower();
                if(type.Contains("farm"))
                    return Brushes.Yellow;
                if(type.Contains("water"))
                    return Brushes.Blue;
                if(type.Contains("power"))
                    return Brushes.Red;
                if(type.Contains("house"))
                    return Brushes.Green;
                else
                    return Brushes.Gray;
                
                
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
