using System.Threading.Tasks;
using cqrs.core.commands;
using cqrs.core.events;

namespace cqrs.core.bus
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
