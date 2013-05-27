// --------------------------------------------------------------------------------------------------------------------
// <copyright file="PropertyObserver.cs" company="Roche">
//   Copyright © Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Monitors the PropertyChanged event of an object that implements INotifyPropertyChanged,
//   and executes callback methods (i.e. handlers) registered for properties of that object.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiCop.UI.MvvmFoundation
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using System.Diagnostics;
	using System.Linq;
	using System.Linq.Expressions;
	using System.Reflection;
	using System.Windows;

	/// <summary>
    /// Monitors the PropertyChanged event of an object that implements INotifyPropertyChanged,
    /// and executes callback methods (i.e. handlers) registered for properties of that object.
    /// </summary>
    /// <typeparam name="TPropertySource">The type of object to monitor for property changes.</typeparam>
    public sealed class PropertyObserver<TPropertySource> : IWeakEventListener
        where TPropertySource : INotifyPropertyChanged
    {
		private readonly Dictionary<string, Action<TPropertySource>> _propertyNameToHandlerMap;
		private readonly WeakReference _propertySourceRef;

		/// <summary>
        /// Initializes a new instance of PropertyObserver, which
        /// observes the 'propertySource' object for property changes.
        /// </summary>
        /// <param name="propertySource">The object to monitor for property changes.</param>
        public PropertyObserver(TPropertySource propertySource)
        {
            if (propertySource == null)
                throw new ArgumentNullException("propertySource");

            this._propertySourceRef = new WeakReference(propertySource);
            this._propertyNameToHandlerMap = new Dictionary<string, Action<TPropertySource>>();
        }

		bool IWeakEventListener.ReceiveWeakEvent(Type managerType, object sender, EventArgs e)
		{
			bool handled = false;

			if (managerType == typeof (PropertyChangedEventManager))
			{
				var args = e as PropertyChangedEventArgs;
				if (args != null && sender is TPropertySource)
				{
					string propertyName = args.PropertyName;
					var propertySource = (TPropertySource) sender;

					if (string.IsNullOrEmpty(propertyName))
					{
						// When the property name is empty, all properties are considered to be invalidated.
						// Iterate over a copy of the list of handlers, in case a handler is registered by a callback.
						foreach (var handler in this._propertyNameToHandlerMap.Values.ToArray())
							handler(propertySource);

						handled = true;
					}
					else
					{
						Action<TPropertySource> handler;
						if (this._propertyNameToHandlerMap.TryGetValue(propertyName, out handler))
						{
							handler(propertySource);

							handled = true;
						}
					}
				}
			}

			return handled;
		}

		/// <summary>
        /// Registers a callback to be invoked when the PropertyChanged event has been raised for the specified property.
        /// </summary>
        /// <param name="expression">A lambda expression like 'n => n.PropertyName'.</param>
        /// <param name="handler">The callback to invoke when the property has changed.</param>
        /// <returns>The object on which this method was invoked, to allow for multiple invocations chained together.</returns>
        public PropertyObserver<TPropertySource> RegisterHandler(
            Expression<Func<TPropertySource, object>> expression, 
            Action<TPropertySource> handler)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            string propertyName = GetPropertyName(expression);
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("'expression' did not provide a property name.");

            if (handler == null)
                throw new ArgumentNullException("handler");

            TPropertySource propertySource = this.GetPropertySource();
            if (propertySource != null)
            {
                Debug.Assert(!this._propertyNameToHandlerMap.ContainsKey(propertyName), 
                             "Why is the '" + propertyName + "' property being registered again?");

                this._propertyNameToHandlerMap[propertyName] = handler;
                PropertyChangedEventManager.AddListener(propertySource, this, propertyName);
            }

            return this;
        }



        /// <summary>
        /// Removes the callback associated with the specified property.
        /// </summary>
        /// <param name="propertyName">A lambda expression like 'n => n.PropertyName'.</param>
        /// <returns>The object on which this method was invoked, to allow for multiple invocations chained together.</returns>
        public PropertyObserver<TPropertySource> UnregisterHandler(Expression<Func<TPropertySource, object>> expression)
        {
            if (expression == null)
                throw new ArgumentNullException("expression");

            string propertyName = GetPropertyName(expression);
            if (string.IsNullOrEmpty(propertyName))
                throw new ArgumentException("'expression' did not provide a property name.");

            TPropertySource propertySource = this.GetPropertySource();
            if (propertySource != null)
            {
                if (this._propertyNameToHandlerMap.ContainsKey(propertyName))
                {
                    this._propertyNameToHandlerMap.Remove(propertyName);
                    PropertyChangedEventManager.RemoveListener(propertySource, this, propertyName);
                }
            }

            return this;
        }

		private static string GetPropertyName(Expression<Func<TPropertySource, object>> expression)
        {
            var lambda = expression as LambdaExpression;
            MemberExpression memberExpression;
            if (lambda.Body is UnaryExpression)
            {
                var unaryExpression = lambda.Body as UnaryExpression;
                memberExpression = unaryExpression.Operand as MemberExpression;
            }
            else
            {
                memberExpression = lambda.Body as MemberExpression;
            }

            Debug.Assert(memberExpression != null, "Please provide a lambda expression like 'n => n.PropertyName'");

            if (memberExpression != null)
            {
                var propertyInfo = memberExpression.Member as PropertyInfo;

                return propertyInfo.Name;
            }

            return null;
        }



        private TPropertySource GetPropertySource()
        {
            try
            {
                return (TPropertySource) this._propertySourceRef.Target;
            }
            catch
            {
                return default(TPropertySource);
            }
        }
    }
}