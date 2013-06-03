namespace ArchiMeter.UI.Support
{
	using System;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows;

	using Autofac;

	using FirstFloor.ModernUI;
	using FirstFloor.ModernUI.Windows;

	public class ModernContentLoader : IContentLoader
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
		protected object LoadContent(Uri uri)
		{
			var content = this.GetContent(uri);
			var element = content as FrameworkElement;
			if (element != null)
			{
				var dataContext = this.GetContext(content);
				element.DataContext = dataContext;
			}

			return content;
		}

		private object GetContent(Uri uri)
		{
			return ModernUIHelper.IsInDesignMode
				? null
				: Application.LoadComponent(uri);
		}

		public async Task<object> LoadContentAsync(Uri uri, CancellationToken cancellationToken)
		{
			var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
			var content = await Task.Factory.StartNew(() => this.GetContent(uri), cancellationToken, TaskCreationOptions.None, scheduler);
			var element = content as FrameworkElement;
			if (element != null)
			{
				var context = this.GetContext(content);
				await Task.Factory.StartNew(() => element.DataContext = context, cancellationToken, TaskCreationOptions.None, scheduler);
			}

			return content;
		}

		private object GetContext(object content)
		{
			var dataContext = content.GetType()
									 .GetCustomAttributes(typeof(DataContextAttribute), true)
									 .OfType<DataContextAttribute>()
									 .FirstOrDefault();
			if (dataContext != null)
			{
				var context = this._container.Resolve(dataContext.DataContextType);
				return context;
			}
			return null;
		}
	}
}
