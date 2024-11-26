using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;
using Test1.Consts;

namespace Test1.Converters;

public class CaseStateToColorConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is CaseState caseState)
        {
            return caseState switch
            {
                CaseState.None => Brushes.White,
                CaseState.Pass => Brushes.LightGreen,
                CaseState.Fail => Brushes.Red,
                _ => Brushes.White
            };
        }
        return Brushes.White;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}