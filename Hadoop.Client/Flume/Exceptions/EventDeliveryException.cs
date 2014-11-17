using System;

namespace Hadoop.Client.Flume.Exceptions
{
    class EventDeliveryException : Exception
    {
        public EventDeliveryException(string message) : base(message)
        {
            
        }
    }
}
