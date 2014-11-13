using System.Collections.Generic;

namespace Hadoop.Client.Hdfs.WebHdfs.Models
{
    public class WebHDFSResponse
    {
        public RemoteExceptionModel RemoteException;
    }

    public class RemoteExceptionModel
    {
        public string exception;
        public string message;
    }
}
