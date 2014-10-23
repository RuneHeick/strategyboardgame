using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Player.ViewModel.Tabs
{
    public class ResearchTab:TabBase
    {

        

        public ObservableCollection<ResearchItem> ResearchItems { get; set; }
        public ObservableCollection<ResearchItem> ResearchQueue { get; set; }
        


        public ResearchTab()
        {
            Name = "Research";
            ResearchItems = new ObservableCollection<ResearchItem>();

            ResearchItems.Add(new ResearchItem("Attack", ""));
            ResearchItems.Add(new ResearchItem("Defence", ""));
            ResearchItems.Add(new ResearchItem("Production", ""));
            ResearchItems.Add(new ResearchItem("Knowlage", ""));

            ResearchQueue = new ObservableCollection<ResearchItem>();
            ResearchQueue.Add(new ResearchItem("default", ""));
            ResearchQueue.Add(new ResearchItem("default", ""));
            ResearchQueue.Add(new ResearchItem("default", ""));
            ResearchQueue.Add(new ResearchItem("default", ""));
            ResearchQueue.Add(new ResearchItem("default", ""));
           
        }









        public class ResearchItem: INotifyPropertyChanged
        {
            public ResearchItem(string name, string description)
            {
                Name = name;
                Description = description; 
            }

            private string name; 
            public string Name
            {
                get
                {
                    return name; 
                }
                set
                {
                    name = value;
                    OnPropertyChanged("Name");
                }
            }
            public string Description { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            protected virtual void OnPropertyChanged(string propertyName)
            {
                PropertyChangedEventHandler handler = this.PropertyChanged;
                if (handler != null)
                {
                    var e = new PropertyChangedEventArgs(propertyName);
                    handler(this, e);
                }
            }
        }


    }
}
