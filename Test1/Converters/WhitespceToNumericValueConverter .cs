using System.Globalization;
using System.Windows.Data;

namespace Test1.Converters
{
    public class WhitespceToNumericValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
      => value;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
          => value is string input && string.IsNullOrWhiteSpace(input)
            ? 0
            : value;
    }
}
