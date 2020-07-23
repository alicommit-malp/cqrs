using RabbitMQ.Client;

namespace cqrs.rabbitMq.infra.rabbitMq
{
    public interface IRabbitConnectionService
    {
        ConnectionFactory CreatePublishingConnection();
        ConnectionFactory CreateConsumingConnection();
    }
}