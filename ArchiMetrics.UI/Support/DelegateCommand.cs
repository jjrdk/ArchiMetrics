namespace ArchiMetrics.UI.Support
{
	using System;
	using System.Windows.Input;
	using System.Windows.Threading;

	internal class DelegateCommand : ICommand
	{
		private readonly Func<object, bool> _canExecute;
		private readonly Action<object> _execute;

		public DelegateCommand(Func<object, bool> canExecute, Action<object> execute)
		{
			_canExecute = canExecute;
			_execute = execute;
		}

		public event EventHandler CanExecuteChanged;

		public bool CanExecute(object parameter)
		{
			return _canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			if (_canExecute(parameter))
			{
				_execute(parameter);
			}
		}

		public void UpdateCanExecute()
		{
			var handler = CanExecuteChanged;
			if (handler != null)
			{
				Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() => handler(this, EventArgs.Empty)));
			}
		}
	}
}
