using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class EventBroker : IEventBroker
{
	private Dictionary<Type, List<Delegate>> subscriptions = new Dictionary<Type, List<Delegate>>();

	public void Publish<T>(object sender, T payload)
	{
		if (sender == null)
		{
			return;
		}

		Type type = typeof(T);

		if (subscriptions.ContainsKey(type))
		{
			for(int i = 0; i < subscriptions[type].Count; i++)
			{
				BrokerEvent<T> brokerEvent = new BrokerEvent<T>(sender, payload);
				subscriptions[type][i]?.DynamicInvoke(brokerEvent);
			}
		}
	}

	public void Subscribe<T>(Action<BrokerEvent<T>> subscription)
	{
		Type type = typeof(T);
		if (subscriptions.ContainsKey(type))
		{
			subscriptions[type].Add(subscription);
		}
		else
		{
			subscriptions.Add(type, new List<Delegate>() { subscription });
		}
	}

	public void Unsubscribe<T>(Action<BrokerEvent<T>> subscription)
	{
		Type type = typeof(T);
		if (subscriptions.ContainsKey(type))
		{
			subscriptions[type].Remove(subscription);

			if (subscriptions[type].Count == 0)
			{
				subscriptions.Remove(type);
			}
		}
	}
}

public class EventBrokerComponent : IEventBroker
{
	private static EventBroker broker;

	public EventBrokerComponent()
	{
		if (broker == null)
		{
			broker = new();
		}
	}

	public void Publish<T>(object sender, T payload)
	{
		broker.Publish<T>(sender, payload);
	}

	public void Subscribe<T>(Action<BrokerEvent<T>> subscription)
	{
		broker.Subscribe<T>(subscription);
	}

	public void Unsubscribe<T>(Action<BrokerEvent<T>> subscription)
	{
		broker.Unsubscribe<T>(subscription);
	}
}