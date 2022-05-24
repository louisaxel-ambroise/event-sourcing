using System;

namespace EventSourcing.MVP.Infrastructure.Exceptions;

public class UnprocessableEventException : Exception
{
    public UnprocessableEventException(string eventType) : base($"Event doesn't have a handler: {eventType}")
    {
    }
}
