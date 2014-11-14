using System;
using Newtonsoft.Json;

namespace Hadoop.Client.Hdfs.WebHdfs.Models
{
    public class WebHDFSResponse
    {
        [JsonProperty("boolean")]
        public bool BooleanValue;

        [JsonProperty("RemoteException")]
        public RemoteExceptionModel RemoteException;
    }

    public class RemoteExceptionModel
    {
        [JsonProperty("exception")]
        public string HDFSException;

        [JsonProperty("message")]
        public string Message;
    }
}
