using SharedData;
using SharedData.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace Logic
{
    public class UserRec
    {
        public ObservableCollection<TagIntContainor> Rec { get; private set; }
        public DataManager manager;

        public UserRec(DataManager Manager)
        {
            Rec = new ObservableCollection<TagIntContainor>();
            manager = Manager;
            Init();
            manager.CollectionChanged += data_CollectionChanged;
            
        }

        private void Init()
        {
            string[] names = manager.ItemNames;
            foreach(string name in names)
            {
                if (name.Contains("Resources"))
                {
                    TagIntContainor item = manager.GetItem<TagIntContainor>(name);
                    if(item != null && item.Tag == "Resources")
                    {
                        Rec.Add(item);
                    }
                }
            }
        }
        public void RemoveRec(string rec)
        {
            TagIntContainor item = Rec.FirstOrDefault((o) => o.RealName == rec);
            if (item == null)
            {
                lock (Rec)
                {
                    Rec.Remove(item);
                }
            }
        }

        void data_CollectionChanged(string Name, ISharedData item, ChangeType ctype, DataManager manager)
        {
            if (Name.Contains("Resources"))
            {
                TagIntContainor rc = item as TagIntContainor;
                if (rc != null && rc.Tag == "Resources")
                {
                    if (ctype == ChangeType.Added)
                    {
                        TagIntContainor re = Rec.FirstOrDefault((o) => o.Name == Name);
                        if (re == null)
                        {
                            Rec.Add(rc);
                        }
                    }
                    else
                    {
                        RemoveRec(rc.RealName);
                    }
                }

            }
        }

        public void AddRec(string Name, int StartValue)
        {
            TagIntContainor item = Rec.FirstOrDefault((o) => o.RealName == Name);
            if (item == null)
            {
                lock (Rec)
                {
                    TagIntContainor rec = new TagIntContainor(Name, "Resources");
                    Rec.Add(rec);
                    manager.Add(rec);
                    rec.Value = StartValue;
                }
            }
        }

        public bool Use(List<RecDemand> ResourcesDemands)
        {
            lock (Rec)
            {
                List<Action> updates = new List<Action>();

                foreach (RecDemand Rd in ResourcesDemands)
                {
                    TagIntContainor crec = Rec.FirstOrDefault((o) => o.RealName.ToLower() == Rd.Rec.ToLower());
                    if (crec != null)
                    {
                        if (crec.Value >= Rd.Quantity)
                            updates.Add(() => crec.Value = crec.Value - Rd.Quantity);
                        else
                            return false;
                    }
                }

                foreach (Action a in updates)
                {
                    a();
                }
                return true; 
            }

        }

        public void Increase(string Name, int quanitity)
        {
            var rec = Rec.FirstOrDefault((o) => o.RealName == Name);
            if(rec != null)
            {
                rec.Value += quanitity;
            }
            else
            {
                AddRec(Name, quanitity); 
            }
        }

        public static ImageSource GetImage(string RecName)
        {
            string uri = ""; 
            switch(RecName.ToLower())
            {

                case "water":
                    uri = "/SharedLogic;component/icons/rain14.png";
                    break;
                case "game point":
                    uri = "/SharedLogic;component/icons/two169.png";
                    break;
                case "food":
                    uri = "/SharedLogic;component/icons/bread4.png";
                    break;
                case "power":
                    uri = "/SharedLogic;component/icons/bolt1.png";
                    break;
                case "houses":
                    uri = "/SharedLogic;component/icons/house58.png";
                    break;
                    
                default: 
                    return null; 
            }

            BitmapImage bi3 = new BitmapImage();
            bi3.BeginInit();
            bi3.UriSource = new Uri(uri, UriKind.Relative);
            bi3.EndInit();
            return bi3; 
        }

    }

}
