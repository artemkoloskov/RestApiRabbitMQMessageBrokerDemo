using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RestApiRabbitMQMessageBrokerDemo.Domain
{
	public class Response
	{
		public Message Message { get; set; }
		public double TimeToHandle { get; set; }
	}
}
