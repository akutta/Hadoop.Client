using System;
using System.Collections.Generic;
using Hadoop.Client.Flume.Event;
using Hadoop.Client.Flume.Exceptions;

namespace Hadoop.Client.Flume
{
    interface IRpcClient : IDisposable
    {

        /// <summary>
        /// Returns the maximum number of events that may be batched at once by AppendBatch
        /// </summary>
        /// <returns></returns>
        int GetBatchSize();

        /// <summary>
        /// Send a single event to the associated Flume source
        ///     This method blocks until RPC returns or until request times out
        /// </summary>
        /// <param name="newEvent"></param>
        /// <exception cref="EventDeliveryException">An error prevented the event delivery</exception>
        void Append(IEvent newEvent);

        /// <summary>
        /// Send a list of events to the associated Flume source
        ///     This method blocks until RPC returns or until request times out
        /// </summary>
        /// <param name="events"></param>
        /// <exception cref="EventDeliveryException">An error prevented the event delivery</exception>
        void AppendBatch(IEnumerable<IEvent> events);

        /// <summary>
        /// Returns True if this object appears to be in a usable state
        /// Returns False if this object is permanently disabled
        ///     If false is returned, application must call close() to clean up system resources
        /// </summary>
        /// <returns></returns>
        bool IsActive();

        /// <summary>
        /// Should be called by Dispose()
        ///     Immediately closes the client so that it may no longer be used.
        /// </summary>
        /// <exception cref="FlumeException"></exception>
        void Close();
    }
}
