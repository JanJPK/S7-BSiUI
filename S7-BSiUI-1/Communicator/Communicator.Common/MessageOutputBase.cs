using System;
using System.Collections.Generic;
using System.Text;

namespace Communicator.Common
{

    /// <summary>
    ///     Base class for handling all received messages.
    /// </summary>
    public abstract class MessageOutputBase : ICanHandleMessageOutput
    {
        protected readonly bool Logging;

        protected MessageOutputBase(bool logging)
        {
            Logging = logging;
        }

        public abstract void HandleMessage(Dictionary<string, string> message);

        public virtual void Log(string logMessage, Dictionary<string, string> message = null)
        {
            if (Logging)
            {
                var time = DateTime.Now;
                if (message != null)
                {
                    StringBuilder sb = new StringBuilder();
                    foreach (var messageKey in message.Keys)
                    {
                        sb.Append($"{messageKey} ");
                    }

                    logMessage += "\n" + sb;
                }

                Console.WriteLine($"[{time.TimeOfDay}]: {logMessage}");
            }
        }

        public virtual void Log(Exception ex)
        {
            if (Logging)
            {
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                }
                var time = DateTime.Now;
                Console.WriteLine($"[{time.TimeOfDay}]: Exception: {ex.Message}");
            }
        }
    }
}