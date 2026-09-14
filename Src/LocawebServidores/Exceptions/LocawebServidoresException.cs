using System;

namespace LocawebServidores.Exceptions
{
    /// <summary>
    /// Base exception for every failure raised by the SDK, including transport errors
    /// (DNS, TLS, connection refused) and payloads that cannot be deserialized.
    /// </summary>
    public class LocawebServidoresException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocawebServidoresException"/> class.
        /// </summary>
        public LocawebServidoresException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocawebServidoresException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public LocawebServidoresException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocawebServidoresException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The underlying exception.</param>
        public LocawebServidoresException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
