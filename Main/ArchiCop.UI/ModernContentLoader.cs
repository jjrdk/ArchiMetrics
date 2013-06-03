namespace ArchiMeter.UI
{
	using System;
	using System.Linq;
	using System.Windows;

	using Autofac;

	using FirstFloor.ModernUI.Windows;

	public class ModernContentLoader : DefaultContentLoader
	{
		private readonly IContainer _container;

		public ModernContentLoader(IContainer container)
		{
			this._container = container;
		}

		/// <summary>
		/// Loads the content from specified uri.
		/// </summary>
		/// <param name="uri">The content uri</param>
		/// <returns>
		/// The loaded content.
		/// </returns>
		protected override object LoadContent(Uri uri)
		{
			var content = base.LoadContent(uri);
			var element = content as FrameworkElement;
			if (element != null)
			{
				var dataContext = content.GetType()
					.GetCustomAttributes(typeof(DataContextAttribute), true)
					.OfType<DataContextAttribute>()
					.FirstOrDefault();
				if (dataContext != null)
				{
					var context = this._container.Resolve(dataContext.DataContextType);

					element.DataContext = context;
				}
			}
			return content;
		}
	}
}
