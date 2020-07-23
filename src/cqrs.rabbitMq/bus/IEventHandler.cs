using System.Threading.Tasks;
using cqrs.rabbitMq.events;

namespace cqrs.rabbitMq.bus
{
    public interface IEventHandler<in TEvent> : IEventHandler
        where TEvent : Event
    {
        Task Handle(TEvent @event);
    }

    public interface IEventHandler
    {

    }
}
