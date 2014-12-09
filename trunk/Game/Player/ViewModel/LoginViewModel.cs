using Network.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Utility.ViewModel;

namespace Player.ViewModel
{
    class LoginViewModel:ViewModelBase
    {

        public string Ip { get; set; }

        public int Port { get; set; }

        public string Name { get; set; }
        public string Password { get; set; }

        private string errorMsg; 
        public string ErrorMsg
        {
            get
            {
                return errorMsg; 
            }
            set
            {
                errorMsg = value;
                OnPropertyChanged("ErrorMsg");
            }
        }

        public bool IsCreate { get; set; }

        public LoginViewModel()
        {
            Ip = "127.0.0.1";
            Port = 5050; 
        }


        private RelayCommand loginCommand;
        public ICommand LoginCommand
        {
            get
            {
                if (loginCommand == null)
                    loginCommand = new RelayCommand((p) => loginCommandExecute());
                return loginCommand;
            }
        }


        private void loginCommandExecute()
        {
            try
            {
                PlayerData.Instance.Connect(Ip, Port);
                GameClient client = PlayerData.Instance.Client;

                client.Login(Name, Password, IsCreate);
                client.OnLoginComplete += client_OnLoginComplete;
            }
            catch
            {
                ErrorMsg = "Ingen forbindelse til Server";
            }
        }

        void client_OnLoginComplete(bool obj)
        {
            PlayerData.Instance.Client.OnLoginComplete -= client_OnLoginComplete;
            if (obj)
            {
                PlayerData.Instance.SwitchView(new MainViewModel(), ViewPrioity.Background);
            }
            else
                ErrorMsg = "Login forkert";
        }

    }
}
