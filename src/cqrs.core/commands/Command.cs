using System;
using cqrs.core.events;

namespace cqrs.core.commands
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
