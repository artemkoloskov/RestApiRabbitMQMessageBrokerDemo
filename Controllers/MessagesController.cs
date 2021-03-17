using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RestApiRabbitMQMessageBrokerDemo.Domain;
using RestApiRabbitMQMessageBrokerDemo.MessageProcessing;

namespace RestApiRabbitMQMessageBrokerDemo.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class MessagesController : ControllerBase
	{
		private readonly ILogger<MessagesController> _logger;
		private readonly IMessageReceiver _messageReceiver;
		private readonly Sender _sender = new Sender("localhost", "restQueue");

		private List<Message> rabbitQueue = new();

		public MessagesController(ILogger<MessagesController> logger, IMessageReceiver messageReceiver)
		{
			_logger = logger;

			_messageReceiver = messageReceiver;
		}

		[HttpGet]
		public IEnumerable<Message> Get()
		{
			if (rabbitQueue.Count() > 0)
			{
				return rabbitQueue.ToArray();
			}
			else
			{
				Random rng = new Random();

				return Enumerable.Range(1, 5).Select(index =>
				new Message($"{rng.Next(5000)}"))
				.ToArray();
			}
		}

		[HttpPost]
		public IActionResult SendMessage([FromBody] Message message)
		{
			DateTime processingStartTime = DateTime.Now;

			_messageReceiver.HandledMessage = null;

			_sender.SendMessage(message);

			TimeSpan timeToHandle = DateTime.Now - processingStartTime;

			while (_messageReceiver.HandledMessage is null && timeToHandle.TotalMilliseconds < 10000)
			{
				timeToHandle = DateTime.Now - processingStartTime;
			}

			if (_messageReceiver.HandledMessage is null)
			{
				return StatusCode(408);
			}

			timeToHandle = DateTime.Now - processingStartTime;
				Response response = new() { Message = _messageReceiver.HandledMessage, TimeToHandle = timeToHandle.TotalMilliseconds/1000 };

			return Ok(response);
		}
	}
}
