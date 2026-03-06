using System.Globalization;

namespace AloPrefeitoP.Converters;

public class PasswordEyeIconConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        // SenhaVisivel = true -> mostra "olho_fechado" (ação: esconder)
        // SenhaVisivel = false -> mostra "olho" (ação: mostrar)
        var visivel = value is bool b && b;
        return visivel ? "olho_fechado.png" : "olho.png";
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotSupportedException();
}