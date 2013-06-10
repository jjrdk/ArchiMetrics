namespace ArchiMeter.UI.Support
{
	using System;
	using System.Linq;
	using System.Threading;
	using System.Threading.Tasks;
	using System.Windows;
	using System.Windows.Threading;

	using Autofac;

	using FirstFloor.ModernUI;
	using FirstFloor.ModernUI.Windows;

	public class ModernContentLoader : IContentLoader
	{
		private readonly IContainer _container;

		public ModernContentLoader(IContainer container)
		{
			_container = container;
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
			var content = await Task.Factory.StartNew(() => GetContent(uri), cancellationToken, TaskCreationOptions.None, scheduler);
			var element = content as FrameworkElement;
			if (element != null)
			{
				var context = await this.GetContext(content, cancellationToken);
				await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.DataBind, new Action(() => element.DataContext = context));
			}

			return content;
		}

		private async Task<object> GetContext(object content, CancellationToken token)
		{
			var dataContext = await Task.Factory.StartNew(() => content.GetType()
																	   .GetCustomAttributes(typeof(DataContextAttribute), true)
																	   .OfType<DataContextAttribute>()
																	   .FirstOrDefault(),
				token,
				TaskCreationOptions.None,
				TaskScheduler.FromCurrentSynchronizationContext());
			if (dataContext != null)
			{
				var context = _container.Resolve(dataContext.DataContextType);
				return context;
			}
			return null;

		}
	}
}
