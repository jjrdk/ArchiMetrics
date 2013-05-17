// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandViewModel.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Represents an actionable item displayed by a View.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiCop.UI.ViewModel
{
	using System;
	using System.Windows.Input;

	/// <summary>
	/// Represents an actionable item displayed by a View.
	/// </summary>
	public class CommandViewModel : ViewModelBase
	{
		public CommandViewModel(string displayName, ICommand command)
		{
			if (command == null)
			{
				throw new ArgumentNullException("command");
			}

			DisplayName = displayName;
			Command = command;
		}

		public ICommand Command { get; private set; }
	}
}