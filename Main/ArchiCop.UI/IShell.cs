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

namespace ArchiCop.UI
{
	using System.Collections.ObjectModel;
	using FirstFloor.ModernUI.Presentation;
	using ViewModel;

	public interface IShell
    {
        ObservableCollection<LinkGroup> Commands { get; }

        ObservableCollection<WorkspaceViewModel> Workspaces { get; }

        void SetActiveWorkspace(WorkspaceViewModel workspace);
    }
}