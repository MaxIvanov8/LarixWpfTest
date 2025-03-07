using System.Windows.Data;

namespace LarixWpfTest;

public class InverseBooleanConverter : IValueConverter
{
	public object Convert(object? value, Type targetType, object? parameter,
		System.Globalization.CultureInfo culture)
	{
		if (value is bool mean)
			return !mean;
		return 0d;
	}

	public object ConvertBack(object? value, Type targetType, object? parameter,
		System.Globalization.CultureInfo culture)
	{
		return 0d;
	}
}