namespace ArchiMetrics.UI.Converters
{
	using System;
	using System.Globalization;
	using System.Windows;
	using System.Windows.Data;

	internal class LinesOfCodeToThicknessConverter : IValueConverter
	{
		public object Convert(
			object value, 
			Type targetType, 
			object parameter, 
			CultureInfo culture)
		{
			var linesOfCode = (int)value;
			var length = Math.Log(linesOfCode) * 25;
			return new Thickness(Math.Max(1, length), 0, 0, 0);
		}

		public object ConvertBack(
			object value, 
			Type targetType, 
			object parameter, 
			CultureInfo culture)
		{
			throw new NotImplementedException();
		}
	}
}