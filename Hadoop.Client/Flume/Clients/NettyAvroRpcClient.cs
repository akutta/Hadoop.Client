using System;
using System.Collections.Generic;
using Hadoop.Client.Flume.Event;

namespace Hadoop.Client.Flume.Clients
{
    class NettyAvroRpcClient : IRpcClient
    {
        private Uri address;
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public int GetBatchSize()
        {
            throw new NotImplementedException();
        }

        public void Append(IEvent newEvent)
        {
            throw new NotImplementedException();
        }

        public void AppendBatch(IEnumerable<IEvent> events)
        {
            throw new NotImplementedException();
        }

        public bool IsActive()
        {
            throw new NotImplementedException();
        }

        public void Close()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            return "NettyAvroRpcClient { host: " + address.Host + ", port: " + address.Port + " }";
        }
    }
}
