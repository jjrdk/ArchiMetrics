using System;
using System.ComponentModel;

namespace ArchiCop.ViewModel
{
    public interface IViewModelBase : IDisposable, INotifyPropertyChanged
    {
        /// <summary>
        ///     Returns the user-friendly name of this object.
        ///     Child classes can set this property to a new value,
        ///     or override it to determine the value on-demand.
        /// </summary>
        string DisplayName { get; }
    }
}