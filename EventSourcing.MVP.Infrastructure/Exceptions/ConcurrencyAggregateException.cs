using System;

namespace EventSourcing.MVP.Infrastructure.Exceptions;

[Serializable]
public class ConcurrencyAggregateException : Exception
{
    public static readonly ConcurrencyAggregateException Instance = new();
}
