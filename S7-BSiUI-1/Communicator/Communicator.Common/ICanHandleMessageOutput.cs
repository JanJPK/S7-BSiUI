using System;
using System.Collections.Generic;

namespace Communicator.Common
{
    /// <summary>
    ///     Provides method for handling text output - received messages and logging.
    /// </summary>
    public interface ICanHandleMessageOutput
    {
        void HandleMessage(Dictionary<string, string> message);
        void Log(string logMessage, Dictionary<string, string> message = null);
        void Log(Exception ex);
    }
}