// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ViewModelBase.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Base class for all ViewModel classes in the application.
//   It provides support for property change notifications
//   and has a DisplayName property.  This class is abstract.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiCop.UI.ViewModel
{
	using System;
	using System.Diagnostics;

	using ArchiCop.UI.MvvmFoundation;

	using Properties;

	/// <summary>
    /// Base class for all ViewModel classes in the application.
    /// It provides support for property change notifications 
    /// and has a DisplayName property.  This class is abstract.
    /// </summary>
    public abstract class ViewModelBase : ObservableObject, IDisposable
    {
		protected ViewModelBase()
		{
			var type = GetType();
			DisplayName = Strings.ResourceManager.GetString(type.Name + "_DisplayName");
		}

		/// <summary>
        /// Returns the user-friendly name of this object.
        /// Child classes can set this property to a new value,
        /// or override it to determine the value on-demand.
        /// </summary>
        public string DisplayName { get; protected set; }

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				// Dispose of any managed resources here. If this class contains unmanaged resources, dispose of them outside of this block. If this class derives from an IDisposable class, wrap everything you do in this method in a try-finally and call base.Dispose in the finally.
				
			}
		}

		~ViewModelBase()
		{
			// Simply call Dispose(false).
			Debug.WriteLine("{0} ({1}) ({2}) Finalized", GetType().Name, DisplayName, GetHashCode());
			Dispose(false);
		}
    }
}