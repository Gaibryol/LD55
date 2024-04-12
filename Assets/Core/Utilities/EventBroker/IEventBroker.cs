using System;

public interface IEventBroker
{
	void Publish<T>(object sender, T payload);
	void Subscribe<T>(Action<BrokerEvent<T>> subscription);
	void Unsubscribe<T>(Action<BrokerEvent<T>> subscription);
}