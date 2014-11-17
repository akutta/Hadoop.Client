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

        [JsonProperty("FileStatus")]
        public FileStatusModel FileStatus;
    }

    public class FileStatusModel
    {
        [JsonProperty("unixTime")]
        public long AccessTime;

        public DateTime AccessTimeDateTime
        {
            get { return ToDateTime(AccessTime); }
        }

        [JsonProperty("blockSize")]
        public long BlockSize;

        [JsonProperty("childrenNum")]
        public int ChildrenNum;

        [JsonProperty("fileId")]
        public long FileId;

        [JsonProperty("group")]
        public string Group;

        [JsonProperty("length")]
        public long Length;

        [JsonProperty("modificationTime")]
        public long LastModified;

        public DateTime LastModifiedDateTime
        {
            get { return ToDateTime(LastModified); }
        }

        [JsonProperty("owner")]
        public string Owner;

        [JsonProperty("pathSuffix")]
        public string PathSuffix;

        [JsonProperty("permission")]
        public string Permission;

        [JsonProperty("replication")]
        public int Replication;

        [JsonProperty("type")]
        public string Type;


        public long LengthKB
        {
            get { return Length/1024; }
        }

        public long LengthMB
        {
            get { return LengthKB/1024; }
        }
        public long LengthGB
        {
            get { return LengthMB/1024; }
        }


        private DateTime ToDateTime(long unixTime)
        {
            return new DateTime(1970, 1, 1).AddMilliseconds(unixTime);
        }

    }

    public class RemoteExceptionModel
    {
        [JsonProperty("exception")]
        public string HDFSException;

        [JsonProperty("message")]
        public string Message;
    }
}
