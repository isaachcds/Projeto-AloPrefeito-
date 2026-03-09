using System.Globalization;

namespace AloPrefeitoP.Converters
{
    public class TextoBuscaHighlightConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var trecho = values.ElementAtOrDefault(0)?.ToString() ?? string.Empty;
            var termo = values.ElementAtOrDefault(1)?.ToString() ?? string.Empty;

            var formatted = new FormattedString();

            if (string.IsNullOrWhiteSpace(trecho))
            {
                formatted.Spans.Add(new Span { Text = string.Empty });
                return formatted;
            }

            if (string.IsNullOrWhiteSpace(termo))
            {
                formatted.Spans.Add(new Span
                {
                    Text = trecho,
                    TextColor = Color.FromArgb("#6F6F6F")
                });
                return formatted;
            }

            int start = 0;
            while (start < trecho.Length)
            {
                var index = trecho.IndexOf(termo, start, StringComparison.OrdinalIgnoreCase);

                if (index < 0)
                {
                    formatted.Spans.Add(new Span
                    {
                        Text = trecho[start..],
                        TextColor = Color.FromArgb("#6F6F6F")
                    });
                    break;
                }

                if (index > start)
                {
                    formatted.Spans.Add(new Span
                    {
                        Text = trecho[start..index],
                        TextColor = Color.FromArgb("#6F6F6F")
                    });
                }

                formatted.Spans.Add(new Span
                {
                    Text = trecho.Substring(index, termo.Length),
                    TextColor = Color.FromArgb("#1B1B1B"),
                    BackgroundColor = Color.FromArgb("#FFF3A3"),
                    FontAttributes = FontAttributes.Bold
                });

                start = index + termo.Length;
            }

            return formatted;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}