namespace ArchiMetrics.UI.Converters
{
	using System;
	using System.Globalization;
	using System.Windows.Data;
	using System.Windows.Media;

	internal class CircularToBrushConverter : IValueConverter
	{
		public object Convert(
			object value, 
			Type targetType, 
			object parameter, 
			CultureInfo culture)
		{
			var isCircular = (bool)value;
			return isCircular ? Brushes.LightPink : Brushes.White;
		}

		public object ConvertBack(
			object value, 
			Type targetType, 
			object parameter, 
			CultureInfo culture)
		{
			throw new NotImplementedException();
		}

		private Brush GetBrush(Brush source)
		{
			var brush = source.Clone();
			brush.Opacity = 0.7;
			brush.Freeze();

			return brush;
		}
	}
}