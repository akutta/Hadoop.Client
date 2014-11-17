﻿using System;
using System.Collections.Generic;
using Hadoop.Client.Flume.Event;
using Hadoop.Client.Flume.Exceptions;

namespace Hadoop.Client.Flume.Clients
{
    class NettyAvroRpcClient : AbstractRpcClient
    {

        protected override void Configure(ClientConfiguration properties)
        {
            AssertNotConfigured();
            Connect();
        }

        public override void Append(IEvent newEvent)
        {
            throw new System.NotImplementedException();
        }

        public override void AppendBatch(IEnumerable<IEvent> events)
        {
            try
            {
                AppendBatch(events, RequestTimeout);
            }
            catch (Exception exception)
            {
                SetState(ConnState.Dead);
                if (exception is TimeoutException)
                {
                    throw new EventDeliveryException("Failed to send event.\tRPC request timed out after " + RequestTimeout + " ms");
                }
                throw new EventDeliveryException("Failed to send batch");
            }
        }

        public override bool IsActive()
        {
            throw new System.NotImplementedException();
        }

        public override void Close()
        {
            throw new System.NotImplementedException();
        }


        private void Connect()
        {
            SetState(ConnState.Ready);


        }

        private void AppendBatch(IEnumerable<IEvent> events, long timeout)
        {
            AssertReady();
        }

    }

}