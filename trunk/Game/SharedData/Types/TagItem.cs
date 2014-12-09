using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData.Types
{

    [Serializable]
    public class TagIntContainor: IntContainor
    {
        public TagIntContainor(string Name, string Tag):base(Name+Tag)
        {
            RealName = Name;
            this.Tag = Tag; 
        }

        public override bool Equals(object obj)
        {
            ISharedData item = obj as ISharedData;
            if (item != null)
                return item.Name == Name;
            return base.Equals(obj);
        }

        public string Tag { get; private set; }

        public string RealName { get; private set; }

    }
}
