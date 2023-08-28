﻿using System.Runtime.Serialization;

namespace Crumb.Core.Parsing;
[Serializable]
public class ParsingException : Exception
{
    public ParsingException() { }

    public ParsingException(string? message) : base(message) { }

    public ParsingException(string? message, Exception? innerException) : base(message, innerException) { }

    protected ParsingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
}