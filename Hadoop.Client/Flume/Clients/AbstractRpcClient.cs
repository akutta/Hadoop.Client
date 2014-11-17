using System;
using System.Collections.Generic;
using Hadoop.Client.Flume.Event;

namespace Hadoop.Client.Flume.Clients
{
    abstract class AbstractRpcClient : IRpcClient
    {
        protected int BatchSize = RpcClientConfigurationConstants.DefaultBatchSize;
        protected long ConnectTimeout = RpcClientConfigurationConstants.DefaultConnectTimeoutMillis;
        protected long RequestTimeout = RpcClientConfigurationConstants.DefaultRequestTimeoutMillis;

        public int GetBatchSize()
        {
            return BatchSize;
        }

        protected abstract void Configure();

        public abstract void Append(IEvent newEvent);

        public abstract void AppendBatch(IEnumerable<IEvent> events);

        public abstract bool IsActive();

        public abstract void Close();


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
