﻿using System;

namespace EventSourcing.MVP.Infrastructure.Exceptions;

[Serializable]
public class BadRequestException : Exception
{
    public BadRequestException(string message) : base(message)
    {
    }
}
