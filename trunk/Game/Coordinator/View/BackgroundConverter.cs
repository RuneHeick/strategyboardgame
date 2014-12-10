using Coordinator.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace Coordinator.View
{
    public class BackgroundConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CollectionItem item = values[0] as CollectionItem;
            if (item != null)
            {

                if (item.Line.Title == values[1])
                    return Brushes.Green;
            }
            return Brushes.Silver;
        }


        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            return null; 
        }
    }
}
