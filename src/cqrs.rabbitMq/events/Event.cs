﻿using System;

 namespace cqrs.rabbitMq.events
{
    public abstract class Event
    {
        private Guid Id { get; }

        private DateTime CreationDate { get; }

        protected Event()
        {
            Id = Guid.NewGuid();
            CreationDate = DateTime.UtcNow;
        }

        protected Event(Guid id, DateTime createDate)
        {
            Id = id;
            CreationDate = createDate;
        }
    }
}
