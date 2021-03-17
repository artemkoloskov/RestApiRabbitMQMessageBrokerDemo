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
		private readonly Sender _sender = new Sender("localhost", "restQueue");

		private List<Message> rabbitQueue = new();

		public MessagesController(ILogger<MessagesController> logger)
		{
			_logger = logger;
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

			_sender.SendMessage(message);

			Thread.Sleep(new Random().Next(5000));

			TimeSpan processingTime = DateTime.Now - processingStartTime;

			return Ok(processingTime.TotalSeconds);
		}
	}
}
