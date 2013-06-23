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

		public Task<object> LoadContentAsync(Uri uri, CancellationToken cancellationToken)
		{
			var scheduler = TaskScheduler.FromCurrentSynchronizationContext();
			return Task.Factory.StartNew(() => GetContent(uri), cancellationToken, TaskCreationOptions.None, scheduler)
					   .ContinueWith(x =>
						   {
							   var content = x.Result;
							   var element = content as FrameworkElement;
							   if (element != null)
							   {
								   var dataContext = content.GetType()
															.GetCustomAttributes(typeof(DataContextAttribute), true)
															.OfType<DataContextAttribute>()
															.FirstOrDefault();
								   GetContext(dataContext)
									   .ContinueWith(t =>
									   {
										   Application.Current.Dispatcher.Invoke(
											   DispatcherPriority.DataBind,
											   new Action(() =>
												   {
													   element.DataContext = t.Result;
												   }));
									   });
							   }

							   return content;
						   });
		}

		private object GetContent(Uri uri)
		{
			return ModernUIHelper.IsInDesignMode
				? null
				: Application.LoadComponent(uri);
		}

		private Task<object> GetContext(DataContextAttribute dataContext)
		{
			return Task.Factory.StartNew(() =>
				{
					if (dataContext != null)
					{
						var context = _container.Resolve(dataContext.DataContextType);
						return context;
					}
					return null;
				});
		}
	}
}
