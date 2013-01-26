using System.Diagnostics;
using MvvmFoundation.Wpf;

namespace ArchiCop.ViewModel
{
    /// <summary>
    ///     Base class for all ViewModel classes in the application.
    ///     It provides support for property change notifications
    ///     and has a DisplayName property.  This class is abstract.
    /// </summary>
    public abstract class ViewModelBase : ObservableObject, IViewModelBase
    {
        #region Constructor

        #endregion // Constructor

        #region DisplayName

        /// <summary>
        ///     Returns the user-friendly name of this object.
        ///     Child classes can set this property to a new value,
        ///     or override it to determine the value on-demand.
        /// </summary>
        public virtual string DisplayName { get; protected set; }

        #endregion // DisplayName

        #region IDisposable Members

        /// <summary>
        ///     Invoked when this object is being removed from the application
        ///     and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            OnDispose();
        }

        /// <summary>
        ///     Child classes can override this method to perform
        ///     clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {
        }

#if DEBUG
        /// <summary>
        ///     Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        ~ViewModelBase()
        {
            string msg = string.Format("{0} ({1}) ({2}) Finalized", GetType().Name, DisplayName, GetHashCode());
            Debug.WriteLine(msg);
        }
#endif

        #endregion // IDisposable Members
    }
}