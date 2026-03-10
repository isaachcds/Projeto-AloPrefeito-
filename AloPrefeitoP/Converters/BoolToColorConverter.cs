using System.Globalization;

namespace AloPrefeitoP.Converters
{
    public class BoolToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var ativo = value is bool b && b;
            return ativo ? Color.FromArgb("#6D74E6") : Color.FromArgb("#C9C9D6");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}