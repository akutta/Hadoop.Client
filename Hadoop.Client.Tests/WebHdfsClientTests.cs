using FluentAssertions;
using Hadoop.Client.Hdfs.WebHdfs;
using NUnit.Framework;

namespace Hadoop.Client.Tests
{
    [TestFixture]
    public class WebHdfsClientTests
    {
        //
        //  Ensure that you have the following entry in your hosts file (Windows:  C:\Windows\System32\drivers\etc\hosts )
        //      127.0.0.1 sandbox.hortonworks.com
        //
        private const string WebHdfsBase = @"http://sandbox.hortonworks.com:50070/";
        private const string FileHdfsPath = "/user/hue/jobsub/sample_data/sonnets.txt";

        [Test]
        public void read_content_via_web_hdfs()
        {
            var hdfsClient = new WebHdfsHttpClient(Connect.WithTestUser(to: WebHdfsBase));
            var file = hdfsClient.OpenFile(FileHdfsPath).Result;

            Assert.IsTrue(file != null, "Null Stream has been Returned.  This particular example expects Hadoop Sandbox v2.1 from Hortonworks");
            Assert.IsTrue(file.Length > 0, "Unable to read file from HDFS");
        }

        [Test]
        public void write_file_stream_to_hdfs()
        {
            var hdfsClient = new WebHdfsHttpClient(Connect.WithTestUser(WebHdfsBase));
            var file = hdfsClient.OpenFile(FileHdfsPath).Result;

            Assert.IsTrue(file.Length > 0);

            file.Position = 0;
            var result = hdfsClient.CreateFile(FileHdfsPath, file, true).Result;
            Assert.IsNull(result.RemoteException);
            Assert.IsNotNullOrEmpty(result.Response);
        }


        [Test]
        public void write_existing_file_to_disk_has_remote_exception()
        {
            var hdfsClient = new WebHdfsHttpClient(Connect.WithTestUser(WebHdfsBase));
            var file = hdfsClient.OpenFile(FileHdfsPath).Result;

            Assert.IsTrue(file.Length > 0);

            file.Position = 0;
            var result = hdfsClient.CreateFile(FileHdfsPath, file, false).Result;

            Assert.IsNotNull(result.RemoteException);
            Assert.IsNullOrEmpty(result.Response);
        }

        [Test]
        public void write_content_via_web_hdfs()
        {
            var hdfsClient = new WebHdfsHttpClient(Connect.WithTestUser(to: WebHdfsBase));

            hdfsClient.CreateDirectory("/abc2").Result.Should().BeTrue();
        }        
    }
}