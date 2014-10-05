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
    public class RecImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            TagIntContainor item = value as TagIntContainor; 
            if(item != null)
            {
                return UserRec.GetImage(item.RealName);
            }
            return null; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException(); 
        }
    }
}
