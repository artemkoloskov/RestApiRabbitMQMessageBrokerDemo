using RestApiRabbitMQMessageBrokerDemo.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiRabbitMQMessageBrokerDemo.MessageProcessing
{
	public interface IMessageReceiver
	{
		Message HandledMessage { get; set; }
	}
}
