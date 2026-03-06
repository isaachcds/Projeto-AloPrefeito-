using System.Globalization;
using Microsoft.Maui.Controls;

namespace AloPrefeitoP.Converters;

public class ChatBubbleAlignConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var isBot = value is bool b && b;
        return isBot ? LayoutOptions.Start : LayoutOptions.End;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}