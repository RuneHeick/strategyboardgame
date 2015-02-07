using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Player.View.Converters
{
    public class AttackConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool i = (bool)values[0];

            Func<bool, bool, bool> func = null;
            string obj = parameter as string;

            if (obj == "&")
                func = (o, b) => o && b;
            if (obj == "|")
                func = (o, b) => o || b;
            if (obj == "x")
                func = (o, b) => o ^ b;

            for (int z = 1; z < values.Length; z++ )
            {
                try
                {
                    if (func != null)
                        i = func(i, (bool)values[z]);
                }
                catch
                {
                }
            }
            return (bool)i;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
