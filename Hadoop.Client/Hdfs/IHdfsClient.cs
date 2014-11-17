using System.IO;
using System.Threading.Tasks;
using Hadoop.Client.Hdfs.WebHdfs;
using Hadoop.Client.Hdfs.WebHdfs.Models;

namespace Hadoop.Client.Hdfs
{
    public interface IHdfsClient
    {
        Task<Stream> OpenFile(string path);

        Task<HadoopResponse> CreateFile(string path, Stream stream, bool overwrite);

        Task<bool> CreateDirectory(string path);

        Task<bool> Delete(string path, bool recursive);

        Task<FileStatusModel> GetFileStatus(string path);        
    }
}
