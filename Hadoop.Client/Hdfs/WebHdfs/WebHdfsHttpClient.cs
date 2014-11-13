﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
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
                var resp = await client.GetAsync(CreateRequestUri(WebHdfsOperation.OPEN, path, null));
                resp.EnsureSuccessStatusCode();
                return await resp.Content.ReadAsStreamAsync();
            }
        }
        
        public async Task<string> CreateFile(string path, Stream content, bool overwrite)
        {           
            var parameters = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("overwrite", overwrite.ToString())
            };

            using (var client = CreateHttpClient())
            {
                var redir = await client.PutAsync(CreateRequestUri(WebHdfsOperation.CREATE, path, parameters), null);

                content.Position = 0;
                var fileContent = new StreamContent(content);
                var create = await client.PutAsync(redir.Headers.Location, fileContent);
                create.EnsureSuccessStatusCode();

                return create.Headers.Location.ToString();
            }
        }
        
        public async Task<bool> Delete(string path, bool recursive)
        {
            using (var client = CreateHttpClient())
            {
                var parameters = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string, string>("recursive", recursive.ToString())
                };
                var drop = await client.DeleteAsync(CreateRequestUri(WebHdfsOperation.DELETE, path, parameters));
                drop.EnsureSuccessStatusCode();

                var content = await drop.Content.ReadAsAsync<JObject>();
                return content.Value<bool>("boolean");
            }
        }
        
        public async Task<DirectoryEntry> GetFileStatus(string path)
        {
            using (var client = CreateHttpClient())
            {
                var status = await client.GetAsync(CreateRequestUri(WebHdfsOperation.GETFILESTATUS, path, null));
                status.EnsureSuccessStatusCode();

                var filesStatusTask = await status.Content.ReadAsAsync<JObject>();

                return new DirectoryEntry(filesStatusTask.Value<JObject>("FileStatus"));
            }
        }

        public async Task<bool> CreateDirectory(string path)
        {
            using (var client = CreateHttpClient())
            {
                var result = await client.PutAsync(CreateRequestUri(WebHdfsOperation.MKDIRS, path, null), null);

                return result.IsSuccessStatusCode;
            }
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
    }
}
