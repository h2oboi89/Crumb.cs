using System.Runtime.Serialization;

namespace Crumb.Core.Lexing
{
    [Serializable]
    internal class LexingException : Exception
    {
        public LexingException() { }

        public LexingException(string? message) : base(message) { }

        public LexingException(string? message, Exception? innerException) : base(message, innerException) { }

        protected LexingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
    }
}