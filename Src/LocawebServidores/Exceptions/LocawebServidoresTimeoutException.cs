using System;

namespace LocawebServidores.Exceptions
{
    /// <summary>
    /// Raised when an HTTP request exceeds the configured timeout, or when waiting
    /// for an asynchronous server action exceeds the caller-supplied limit.
    /// </summary>
    public class LocawebServidoresTimeoutException : LocawebServidoresException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LocawebServidoresTimeoutException"/> class.
        /// </summary>
        public LocawebServidoresTimeoutException() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocawebServidoresTimeoutException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        public LocawebServidoresTimeoutException(string message)
            : base(message) { }

        /// <summary>
        /// Initializes a new instance of the <see cref="LocawebServidoresTimeoutException"/> class.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The underlying exception.</param>
        public LocawebServidoresTimeoutException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
