using RabbitMQ.Client;

namespace cqrs.rabbitMq.infra.rabbitMq
{
    public class RabbitMqDefaultConnectionService :IRabbitConnectionService
    {
        public ConnectionFactory CreatePublishingConnection()
        {
            return new ConnectionFactory()
            {
                HostName = "localhost"
            };
        }

        public ConnectionFactory CreateConsumingConnection()
        {
            return new ConnectionFactory()
            {
                HostName = "localhost",
                DispatchConsumersAsync = true
            };
        }
    }
}