using System.Collections.Generic;

namespace Hadoop.Client.Flume.Event
{
    interface IEvent
    {
        Dictionary<string, string> GetHeaders();
        void SetHeaders(Dictionary<string, string> headers);
        byte[] GetBody();
        void SetBody(byte[] body);
    }
}
