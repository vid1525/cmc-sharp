using System;
using System.Globalization;
using System.ComponentModel;
using System.Windows.Data;

namespace ViewModel
{
    public class StringToDoubleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{(double)value}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return double.Parse((string)value, culture);
        }
    }

    public class StringToIntConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return $"{(int)value}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Int32.Parse((string)value, culture);
        }
    }

    public class StringToFunctionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var val = (Splines.SPf)value;

            if (val == Splines.SPf.CUBIC_FUNC)
            {
                return "Cubic";
            }
            else if (val == Splines.SPf.SQRT)
            {
                return "Sqrt";
            }
            else if (val == Splines.SPf.RANDOM_FUNC)
            {
                return "Random";
            }

            throw new InvalidEnumArgumentException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = ((System.Windows.Controls.TextBlock)value).Text;

            if (str == "Cubic")
            {
                return Splines.SPf.CUBIC_FUNC;
            }
            else if (str == "Sqrt")
            {
                return Splines.SPf.SQRT;
            }
            else if (str == "Random")
            {
                return Splines.SPf.RANDOM_FUNC;
            }

            throw new InvalidEnumArgumentException();
        }
    }
}
