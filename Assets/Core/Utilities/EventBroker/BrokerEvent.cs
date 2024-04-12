using System;

public class BrokerEvent<T>
{
	public object Sender;
	public T Payload;
	public DateTime Timestamp;

	public BrokerEvent(object sender, T payload)
	{
		Sender = sender;
		Payload = payload;
		Timestamp = DateTime.UtcNow;
	}
}