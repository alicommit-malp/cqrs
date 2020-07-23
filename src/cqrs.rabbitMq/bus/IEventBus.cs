using System.Threading.Tasks;
using cqrs.rabbitMq.commands;
using cqrs.rabbitMq.events;

namespace cqrs.rabbitMq.bus
{
    public interface IEventBus
    {
        Task SendCommand<T>(T command) where T : Command;

        void Publish<T>(T @event) where T : Event;

        void Subscribe<T, TH>()
            where T : Event
            where TH : IEventHandler<T>;
    }
}
