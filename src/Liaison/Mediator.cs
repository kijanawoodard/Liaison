using System;
using System.Collections.Generic;

namespace Liaison
{
	public interface ISubscribeHandlers
	{
		void Subscribe<TMessage>(Action<TMessage> handler);
		void Subscribe<TMessage, TResult>(Func<TMessage, TResult> handler);
	}

	public interface IMediator
	{
		void Send<TMessage>(TMessage message);
		TResult Send<TMessage, TResult>(TMessage message);
	}

	public class Mediator : ISubscribeHandlers, IMediator
	{
		private readonly Dictionary<Tuple<Type, Type>, dynamic> _subscriptions;

		public void Subscribe<TMessage>(Action< TMessage> handler)
		{
			Subscribe<TMessage, Unit>(message =>
			{
				handler(message);
				return new Unit();
			});
		}

		public void Subscribe<TMessage, TResult>(Func<TMessage, TResult> handler)
		{
			_subscriptions.Add(Tuple.Create(typeof(TMessage), typeof(TResult)), handler);
		}

		public void Send<TMessage>(TMessage message)
		{
			Send<TMessage, Unit>(message);
		}

		public TResult Send<TMessage, TResult>(TMessage message)
		{
			dynamic handler;
			if (!_subscriptions.TryGetValue(Tuple.Create(typeof(TMessage), typeof(TResult)), out handler))
				throw new ApplicationException(string.Format("No Handler subscribed for message {0} with return type of {1}.", typeof(TMessage).Name, typeof(TResult).Name));

			return (TResult)handler(message);
		}

		public Mediator()
		{
			_subscriptions = new Dictionary<Tuple<Type, Type>, dynamic>();
		}

		class Unit { }
	}
}