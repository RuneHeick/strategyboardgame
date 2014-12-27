using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedData
{
    public interface ISharedData : INotifyPropertyChanged
    {
        string Name { get; }

        void Update(ISharedData data);

    }
}
