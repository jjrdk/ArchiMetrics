// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RelayCommand.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   A command whose sole purpose is to
//   relay its functionality to other
//   objects by invoking delegates. The
//   default return value for the CanExecute
//   method is 'true'.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.MvvmFoundation
{
	using System;
	using System.Diagnostics;
	using System.Windows.Input;

	/// <summary>
	/// A command whose sole purpose is to 
	/// relay its functionality to other
	/// objects by invoking delegates. The
	/// default return value for the CanExecute
	/// method is 'true'.
	/// </summary>
	public class RelayCommand<T> : ICommand
	{
		private readonly Predicate<T> _canExecute;
		private readonly Action<T> _execute;

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public RelayCommand(Action<T> execute, Predicate<T> canExecute = null)
		{
			if (execute == null)
			{
				throw new ArgumentNullException("execute");
			}

			_execute = execute;
			_canExecute = canExecute;
		}

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute((T)parameter);
		}

		public event EventHandler CanExecuteChanged
		{
			add
			{
				if (_canExecute != null)
				{
					CommandManager.RequerySuggested += value;
				}
			}

			remove
			{
				if (_canExecute != null)
				{
					CommandManager.RequerySuggested -= value;
				}
			}
		}

		public void Execute(object parameter)
		{
			_execute((T)parameter);
		}
	}

	/// <summary>
	/// A command whose sole purpose is to 
	/// relay its functionality to other
	/// objects by invoking delegates. The
	/// default return value for the CanExecute
	/// method is 'true'.
	/// </summary>
	public class RelayCommand : ICommand
	{
		private readonly Func<bool> _canExecute;
		private readonly Action _execute;

		/// <summary>
		/// Creates a new command.
		/// </summary>
		/// <param name="execute">The execution logic.</param>
		/// <param name="canExecute">The execution status logic.</param>
		public RelayCommand(Action execute, Func<bool> canExecute = null)
		{
			if (execute == null)
				throw new ArgumentNullException("execute");

			_execute = execute;
			_canExecute = canExecute;
		}

		[DebuggerStepThrough]
		public bool CanExecute(object parameter)
		{
			return _canExecute == null || _canExecute();
		}

		public event EventHandler CanExecuteChanged
		{
			add
			{
				if (_canExecute != null)
					CommandManager.RequerySuggested += value;
			}

			remove
			{
				if (_canExecute != null)
					CommandManager.RequerySuggested -= value;
			}
		}

		public void Execute(object parameter)
		{
			_execute();
		}
	}
}
