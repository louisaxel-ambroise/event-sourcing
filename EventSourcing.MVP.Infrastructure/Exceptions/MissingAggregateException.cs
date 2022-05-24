using System;

namespace EventSourcing.MVP.Infrastructure.Exceptions;

public class MissingAggregateException : Exception
{
    public MissingAggregateException(string aggregateName, string aggregateId) : base($"Missing Aggregate: {aggregateName}#{aggregateId}")
    {
    }
}
