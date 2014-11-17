using System;
using System.Collections.Generic;
using Hadoop.Client.Flume.Event;
using Hadoop.Client.Flume.Exceptions;

namespace Hadoop.Client.Flume.Clients
{
    internal enum ConnState
    {
        Ready,
        Dead
    }

    abstract class AbstractRpcClient : IRpcClient
    {
        protected int BatchSize = RpcClientConfigurationConstants.DefaultBatchSize;
        protected long ConnectTimeout = RpcClientConfigurationConstants.DefaultConnectTimeoutMillis;
        protected long RequestTimeout = RpcClientConfigurationConstants.DefaultRequestTimeoutMillis;

        private readonly object _stateLock = new object();
        private ConnState _connState;

        public int GetBatchSize()
        {
            return BatchSize;
        }

        protected abstract void Configure(ClientConfiguration properties);

        public abstract void Append(IEvent newEvent);

        public abstract void AppendBatch(IEnumerable<IEvent> events);

        public abstract bool IsActive();

        public abstract void Close();

        protected void AssertReady()
        {
            lock (_stateLock)
            {
                var curState = _connState;
                if (curState != ConnState.Ready)
                {
                    throw new EventDeliveryException("RPC failed, client in an invalid state: " + curState);
                }
            }
        }

        protected void AssertNotConfigured()
        {
            lock (_stateLock)
            {
                var curState = _connState;
                if ( curState == ConnState.Ready || curState == ConnState.Dead ) 
                    throw new FlumeException("This client was already configured, cannot reconfigure");
            }
        }

        protected void SetState(ConnState newState)
        {
            lock (_stateLock)
            {
                if (_connState == ConnState.Dead && _connState != newState)
                {
                    throw new IllegalStateException("Cannot transition from CLOSED state.");
                }

                _connState = newState;
            }
        }

        public void Dispose()
        {
            Close();
        }

    }

    internal class RpcClientConfigurationConstants
    {
        public static string ConfigHosts = "hosts";
        public static string ConfigHostsPrefix = "hosts.";
        public static string ConfigBatchSize = "batch-size";
        public static string ConfigConnectTimeout = "connect-timeout";
        public static string ConfigRequestTimeout = "request-timeout";
        public static int DefaultBatchSize = 100;
        public static long DefaultConnectTimeoutMillis = 1000*20;
        public static long DefaultRequestTimeoutMillis = 1000*20;

        public static string ConfigMaxAttempts = "max-attempts";
        public static string ConfigClientType = "client.types";

    }
}
