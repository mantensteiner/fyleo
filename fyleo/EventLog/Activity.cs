using System;

namespace fyleo.EventLog
{
    public class Activity
    {
        public readonly DateTime Timestamp;
        public readonly string User;
        public readonly string Action;
        public readonly string Message;

        private Activity(DateTime timestamp, string user, string action, string message)
        {
            Timestamp = timestamp;
            User = user;
            Action = action;
            Message = message;
        }


        public static Activity Parse(DateTime timestamp, string user, string action, string message)
        {
            return new Activity(timestamp, user, action, message);
        }

    }
}