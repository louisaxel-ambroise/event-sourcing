using System;

namespace EventSourcing.MVP.Infrastructure.Exceptions;

[Serializable]
public class ConcurrencyAggregateException : Exception
{
}
