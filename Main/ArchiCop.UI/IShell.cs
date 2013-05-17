// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IShell.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the IShell type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.UI
{
	using System.Collections.ObjectModel;
	using ArchiCop.UI.ViewModel;

	public interface IShell
    {
        ObservableCollection<CommandViewModel> Commands { get; }

        ObservableCollection<WorkspaceViewModel> Workspaces { get; }

        void SetActiveWorkspace(WorkspaceViewModel workspace);
    }
}