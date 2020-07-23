using System;
using cqrs.rabbitMq.events;

namespace cqrs.rabbitMq.commands
{
    public abstract class Command : Message
    {
        private Guid Id { get; }

        private DateTime CreationDate { get; }

        protected Command()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        protected Command(Guid id, DateTime createDate)
        {
            Id = id;
            CreationDate = createDate;
        }
    }
}
