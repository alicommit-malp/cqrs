using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using cqrs.rabbitMq.bus;
using cqrs.rabbitMq.commands;
using cqrs.rabbitMq.events;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace cqrs.rabbitMq.infra.rabbitMq
{
    // ReSharper disable once InconsistentNaming
    public sealed class RabbitMQBus : IEventBus
    {
        private readonly IMediator _mediator;
        private readonly Dictionary<string, List<Type>> _handlers;
        private readonly List<Type> _eventTypes;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IRabbitConnectionService _rabbitConnectionService;

        public RabbitMQBus(IMediator mediator, IServiceScopeFactory serviceScopeFactory,
            IRabbitConnectionService rabbitConnectionService)
        {
            _mediator = mediator;
            _scopeFactory = serviceScopeFactory;
            _rabbitConnectionService = rabbitConnectionService ?? new RabbitMqDefaultConnectionService(); 
            _handlers = new Dictionary<string, List<Type>>();
            _eventTypes = new List<Type>();
        }

        public Task SendCommand<T>(T command) where T : Command
        {
            return _mediator.Send(command);
        }

        public void Publish<T>(T @event) where T : Event
        {
            var factory = _rabbitConnectionService.CreatePublishingConnection();

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            var eventName = @event.GetType().Name;

            channel.QueueDeclare(eventName, false, false, false, null);

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish("", eventName, null, body);
        }

        public void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>
        {
            var eventName = typeof(T).Name;
            var handlerType = typeof(TH);

            if (!_eventTypes.Contains(typeof(T)))
            {
                _eventTypes.Add(typeof(T));
            }

            if (!_handlers.ContainsKey(eventName))
            {
                _handlers.Add(eventName, new List<Type>());
            }

            if (_handlers[eventName].Any(s => s == handlerType))
            {
                throw new ArgumentException(
                    $"Handler Type {handlerType.Name} already is registered with '{eventName}'", nameof(handlerType));
            }

            _handlers[eventName].Add(handlerType);

            StartBasicConsume<T>();
        }

        private void StartBasicConsume<T>() where T : Event
        {
            var connectionFactory = _rabbitConnectionService.CreateConsumingConnection();

            var connection = connectionFactory.CreateConnection();
            var channel = connection.CreateModel();

            var eventName = typeof(T).Name;

            channel.QueueDeclare(eventName, false, false, false, null);

            var eventingBasicConsumer = new AsyncEventingBasicConsumer(channel);
            eventingBasicConsumer.Received += Consumer_Received;

            channel.BasicConsume(eventName, true, eventingBasicConsumer);
        }

        private async Task Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var routingKey = e.RoutingKey;
            var message = Encoding.UTF8.GetString(e.Body.ToArray());

            try
            {
                await ProcessEvent(routingKey, message).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // ignored
            }
        }

        private async Task ProcessEvent(string eventName, string message)
        {
            if (_handlers.ContainsKey(eventName))
            {
                using var serviceScope = _scopeFactory.CreateScope();
                var subscriptions = _handlers[eventName];
                foreach (var subscription in subscriptions)
                {
                    var service = serviceScope.ServiceProvider.GetService(subscription);
                    if (service == null) continue;
                    var eventType = _eventTypes.SingleOrDefault(t => t.Name == eventName);
                    var @event =
                        JsonConvert.DeserializeObject(message, eventType ?? throw new InvalidOperationException());
                    var genericType = typeof(IEventHandler<>).MakeGenericType(eventType);
                    // ReSharper disable once PossibleNullReferenceException
                    await (Task) genericType.GetMethod("Handle")?.Invoke(service, new[] {@event});
                }
            }
        }
    }
}