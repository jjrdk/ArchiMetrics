// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EventAggregator.cs" company="Reimers.dk">
//   Copyright © Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the EventAggregator type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.Support
{
	using System;
	using System.Reactive.Subjects;
	using System.Threading.Tasks;
	using Messages;

	internal class EventAggregator : IObservable<IMessage>, IDisposable
	{
		private readonly Subject<IMessage> _messageSubject = new Subject<IMessage>();

		public Task Publish(IMessage message)
		{
			return Task.Factory.StartNew(() => _messageSubject.OnNext(message));
		}

		/// <summary>
		/// Notifies the provider that an observer is to receive notifications.
		/// </summary>
		/// <returns>
		/// A reference to an interface that allows observers to stop receiving notifications before the provider has finished sending them.
		/// </returns>
		/// <param name="observer">The object that is to receive notifications.</param>
		public IDisposable Subscribe(IObserver<IMessage> observer)
		{
			return _messageSubject.Subscribe(observer);
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			_messageSubject.OnCompleted();
			_messageSubject.Dispose();
		}
	}
}
