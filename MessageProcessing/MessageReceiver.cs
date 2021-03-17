using RestApiRabbitMQMessageBrokerDemo.Domain;

namespace RestApiRabbitMQMessageBrokerDemo.MessageProcessing
{
	public class MessageReceiver : IMessageReceiver
	{
		public Message HandledMessage { get; set; }
	}
}
