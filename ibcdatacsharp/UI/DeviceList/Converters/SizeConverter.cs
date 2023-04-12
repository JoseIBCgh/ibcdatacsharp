using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace ibcdatacsharp.UI.DeviceList.Converters
{
    // No hace falta, te convierte a string automaticamente
    [ValueConversion(typeof(Size), typeof(string))]
    public class SizeConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return string.Empty;
            }
            else
            {
                Size size = (Size)value;
                return size.Height + "x" + size.Width;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string strValue = value as string;
            if (strValue == string.Empty)
            {
                return null;
            }
            else
            {
                string[] subchains = strValue.Split("x");
                int height = int.Parse(subchains[0]);
                int width = int.Parse(subchains[1]);
                return new Size(height, width);
            }
        }
    }
}
