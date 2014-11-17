using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Hadoop.Client.Hdfs.WebHdfs.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Hadoop.Client.Hdfs.WebHdfs
{   
    public class WebHdfsHttpClient : IHdfsClient
    {
        private readonly ConnectionConfig _authCredential;        
        private readonly Uri _webHdfsUri;
            
        public WebHdfsHttpClient(ConnectionConfig authCredential)
        {
            _authCredential = authCredential;
            _webHdfsUri = new Uri(authCredential.Server, "/webhdfs/v1");
        }
        
        public async Task<Stream> OpenFile(string path)
        {
            using (var client = CreateHttpClient())
            {
                var resp = await PerformWebOperation(WebHdfsOperation.OPEN, client, path);
                resp.EnsureSuccessStatusCode();
                return await resp.Content.ReadAsStreamAsync();
            }
        }

        public async Task<HadoopResponse> AppendToFile(string path, string toAppend)
        {
            return await AppendToFile(path, GenerateStreamFromString(toAppend));
        }

        private Stream GenerateStreamFromString(string toAppend)
        {
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);

            writer.Write(toAppend);

            writer.Flush();
            return stream;
        }

        public async Task<HadoopResponse> AppendToFile(string path, Stream stream)
        {
            using (var client = CreateHttpClient(false))
            {
                var requestUri = CreateRequestUri(WebHdfsOperation.APPEND, path, null);
                var redir = await client.PostAsync(requestUri, null);
                var fileContent = GetFileContent(stream);
                var create = client.PostAsync(redir.Headers.Location, fileContent).Result;

                var responseString = string.Empty;
                if (create.Headers.Location != null)
                    responseString = create.Headers.Location.ToString();

                var content = create.Content.ReadAsStringAsync().Result;

                return new HadoopResponse
                {
                    Response = responseString,
                    RemoteException = ParseResponse(content).RemoteException
                };
            }
        }

        public async Task<HadoopResponse> CreateFile(string path, Stream stream, bool overwrite)
        {           
            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("overwrite", overwrite.ToString())
            };

            using (var client = CreateHttpClient(false))
            {
                return await CreateOrAppendFile(WebHdfsOperation.CREATE, path, stream, client, parameters);
            }
        }

        private async Task<HadoopResponse> CreateOrAppendFile(WebHdfsOperation webHdfsOperation, string path, Stream stream, HttpClient client, List<KeyValuePair<string, string>> parameters)
        {
            var redir = await PerformWebOperation(webHdfsOperation, client, path, parameters);

            var fileContent = GetFileContent(stream);

            var create = await PerformWebOperation(webHdfsOperation, client, redir.Headers.Location, fileContent);

            var responseString = string.Empty;
            if (create.Headers.Location != null)
                responseString = create.Headers.Location.ToString();

            var content = create.Content.ReadAsStringAsync().Result;

            return new HadoopResponse
            {
                Response = responseString,
                RemoteException = ParseResponse(content).RemoteException
            };
        }

        private static StreamContent GetFileContent(Stream stream)
        {
            stream.Position = 0;
            var fileContent = new StreamContent(stream);
            return fileContent;
        }


        public async Task<bool> Delete(string path, bool recursive)
        {
            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("recursive", recursive.ToString())
            };
            using (var client = CreateHttpClient())
            {
                var drop = await PerformWebOperation(WebHdfsOperation.DELETE, client, path, parameters);
                drop.EnsureSuccessStatusCode();

                return ParseResponse(drop.Content.ReadAsStringAsync().Result).BooleanValue;
            }
        }
        
        public async Task<DirectoryEntry> GetFileStatus(string path)
        {
            using (var client = CreateHttpClient())
            {
                var status = await PerformWebOperation(WebHdfsOperation.GETFILESTATUS, client, path);
                status.EnsureSuccessStatusCode();

                var filesStatusTask = await status.Content.ReadAsAsync<JObject>();

                return new DirectoryEntry(filesStatusTask.Value<JObject>("FileStatus"));
            }
        }

        public async Task<bool> CreateDirectory(string path)
        {
            using (var client = CreateHttpClient())
            {
                var result =
                    await
                        PerformWebOperation(WebHdfsOperation.MKDIRS, client, path);
                var response = ParseResponse(await result.Content.ReadAsStringAsync());

                return response.BooleanValue;
            }
        }

        private async Task<HttpResponseMessage> PerformWebOperation(WebHdfsOperation webHdfsOperation, HttpClient client, string path, List<KeyValuePair<string, string>> parameters = null, StreamContent fileContent = null)
        {
            var requestUri = CreateRequestUri(webHdfsOperation, path, parameters);
            return await PerformWebOperation(webHdfsOperation, client, requestUri, fileContent);
        }

        private static async Task<HttpResponseMessage> PerformWebOperation(WebHdfsOperation webHdfsOperation, HttpClient client, Uri requestUri, StreamContent fileContent)
        {
            HttpResponseMessage response = null;
            switch (webHdfsOperation)
            {
                case WebHdfsOperation.MKDIRS:
                case WebHdfsOperation.CREATE:
                    response = await client.PutAsync(requestUri, fileContent);
                    break;
                case WebHdfsOperation.APPEND:
                    response = await client.PostAsync(requestUri, fileContent);
                    break;
                case WebHdfsOperation.DELETE:
                    response = await client.DeleteAsync(requestUri);
                    break;
                case WebHdfsOperation.OPEN:
                case WebHdfsOperation.GETFILESTATUS:
                    response = await client.GetAsync(requestUri);
                    break;
            }
            return response;
        }

        private HttpClient CreateHttpClient(bool allowsAutoRedirect = true)
        {
            return HttpClientBuilder.Create(allowsAutoRedirect)
                .WithBasicAuthenticationFrom(_authCredential)
                .AcceptJson()
                .AcceptOctetStream()
                .Build();
        }

        private Uri CreateRequestUri(WebHdfsOperation operation, string path, List<KeyValuePair<string, string>> parameters)
        {
            if (parameters == null)
                parameters = new List<KeyValuePair<string, string>>();

            parameters.Add(new KeyValuePair<string, string>(
                HadoopRemoteRestConstants.UserName,
                _authCredential.UserName.EscapeDataString()));

            string paramString = parameters.Aggregate("",
                (current, param) => current + string.Format("&{0}={1}", param.Key, param.Value));

            var queryString = string.Format("{0}?op={1}{2}", path, operation, paramString);
            
            var uri = new Uri(_webHdfsUri + queryString);
            return uri;
        }

        private static WebHDFSResponse ParseResponse(string reason)
        {
            return String.IsNullOrEmpty(reason) ? new WebHDFSResponse() : JsonConvert.DeserializeObject<WebHDFSResponse>(reason);
        }


    }

    public class HadoopResponse
    {
        public RemoteExceptionModel RemoteException;
        public string Response;

    }
}
