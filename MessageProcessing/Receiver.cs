using System;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RestApiRabbitMQMessageBrokerDemo.Domain;

namespace RestApiRabbitMQMessageBrokerDemo.MessageProcessing
{
	public class Receiver : BackgroundService
	{
		private IModel _channel;
		private IConnection _connection;
		private IMessageReceiver _messageReceiver;
		private readonly string _hostname = "localhost";
		private readonly string _queueName = "restQueue";

		public Receiver(IMessageReceiver messageReceiver)
		{
			_messageReceiver = messageReceiver;

			InitializeListener();
		}

		private void InitializeListener()
		{
			ConnectionFactory factory = new ConnectionFactory
			{
				HostName = _hostname
			};

			_connection = factory.CreateConnection();

			_channel = _connection.CreateModel();

			_channel.QueueDeclare(
				queue: _queueName,
				durable: false,
				exclusive: false,
				autoDelete: false,
				arguments: null);
		}

		protected override Task ExecuteAsync(CancellationToken stoppingToken)
		{
			stoppingToken.ThrowIfCancellationRequested();

			EventingBasicConsumer consumer = new EventingBasicConsumer(_channel);

			consumer.Received += (ch, ea) =>
			{
				string content = Encoding.UTF8.GetString(ea.Body.ToArray());
				Message messageFromQueue = JsonSerializer.Deserialize<Message>(content);

				HandleMessage(messageFromQueue);

				_messageReceiver.HandledMessage = messageFromQueue;

				_channel.BasicAck(
					deliveryTag: ea.DeliveryTag,
					multiple: false);
			};

			_channel.BasicConsume(
				queue: _queueName,
				autoAck: false,
				consumer: consumer);

			return Task.CompletedTask;
		}

		private void HandleMessage(Message message)
		{
			int dots = message.Body.Split('.').Length - 1;

			Thread.Sleep(dots * 1000);

			message.Body = $"Сообщение обработано: {message.Body.Replace(".", "")}, {dots}";
		}

		public override void Dispose()
		{
			_channel.Close();

			_connection.Close();

			base.Dispose();
		}
	}
}
