// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WorkspaceViewModel.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   This ViewModelBase subclass requests to be removed
//   from the UI when its CloseCommand executes.
//   This class is abstract.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.UI.ViewModel
{
	using System;
	using System.Windows.Input;

	using ArchiMeter.UI.MvvmFoundation;

	/// <summary>
	/// This ViewModelBase subclass requests to be removed 
	/// from the UI when its CloseCommand executes.
	/// This class is abstract.
	/// </summary>
	public abstract class WorkspaceViewModel : ViewModelBase
	{
		private RelayCommand<object> _closeCommand;
		private bool _isLoading;

		/// <summary>
		/// Returns the command that, when invoked, attempts
		/// to remove this workspace from the user interface.
		/// </summary>
		public ICommand CloseCommand
		{
			get
			{
				return this._closeCommand ?? (this._closeCommand = new RelayCommand<object>(param => this.OnRequestClose()));
			}
		}

		public bool IsLoading
		{
			get
			{
				return this._isLoading;
			}

			set
			{
				if (this._isLoading != value)
				{
					this._isLoading = value;
					this.RaisePropertyChanged();
				}
			}
		}

		public virtual void Update(bool forceReload)
		{
		}

		/// <summary>
		/// Raised when this workspace should be removed from the UI.
		/// </summary>
		public event EventHandler RequestClose;

		private void OnRequestClose()
		{
			EventHandler handler = this.RequestClose;
			if (handler != null)
			{
				handler(this, EventArgs.Empty);
			}
		}
	}
}