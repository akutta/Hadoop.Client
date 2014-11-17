using System;

namespace Hadoop.Client.Flume.Exceptions
{
    class FlumeException : Exception
    {
        public FlumeException(string message) : base(message)
        {
            
        }
    }
}
