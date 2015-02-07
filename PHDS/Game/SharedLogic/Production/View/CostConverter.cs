using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedData.Types;
using Logic;
using System.Globalization;
using System.Windows.Data;

namespace Player.View.Converters
{
    public class CostConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string s = parameter as string; 
            var l = value as List<BuildingContainor.UseCond>;
            if(l != null )
            {
                var item = l.FirstOrDefault((o)=>o.Resource == s);
                if(item != null)
                    return item.Quantity;
            }

            var ll = value as List<RecDemand>;
            if (ll != null)
            {
                var item = ll.FirstOrDefault((o) => o.Rec == s);
                if (item != null)
                    return item.Quantity;
            }
            return 0; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); 
        }
    }
}
