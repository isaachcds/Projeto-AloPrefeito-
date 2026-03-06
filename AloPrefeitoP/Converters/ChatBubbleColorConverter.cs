using System.Globalization;
using Microsoft.Maui.Graphics;

namespace AloPrefeitoP.Converters;

public class ChatBubbleColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        var isBot = value is bool b && b;

        // Bot: verde/água claro | Usuário: lilás claro
        return isBot ? Color.FromArgb("#EAF7F7") : Color.FromArgb("#DDE1FF");
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}