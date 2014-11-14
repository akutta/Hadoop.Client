﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Hadoop.Client.Hdfs;

namespace Hadoop.Client.Jobs.Hive
{    
    //TODO: Very naive implementation, IMPROVE !!!    
    public class HiveClient
    {
        private readonly IHdfsClient _hdfsClient;
        private readonly IHadoopJobClient _jobClient;
        
        private readonly HiveClientConfig _config;

        public HiveClient(IHdfsClient hdfsClient, IHadoopJobClient jobClient, HiveClientConfig config)
        {
            _hdfsClient = hdfsClient;
            _jobClient = jobClient;
            _config = config;
        }

        public async Task<JobResults> Query(string hiveQuery)
        {
            var jobIdentifier = Guid.NewGuid();
            string path = Path.Combine(_config.ResultsFolderBase, jobIdentifier + ".output");

            var creationResult = await ScheduleNewJob(hiveQuery, path, jobIdentifier);

            var token = new CancellationToken(false);
            await _jobClient.WaitForJobCompletionAsync(creationResult, TimeSpan.FromMinutes(5), token);

            return await ReadResults(path);
        }

        public async Task<IEnumerable<TResult>> Query<TResult>(string hiveQuery, IReadResults<TResult> reader)
        {
            var jobIdentifier = Guid.NewGuid();
            string path = Path.Combine(_config.ResultsFolderBase, jobIdentifier + ".output");

            var creationResult = await ScheduleNewJob(hiveQuery, path, jobIdentifier);

            var token = new CancellationToken(false);
            await _jobClient.WaitForJobCompletionAsync(creationResult, TimeSpan.FromMinutes(5), token);

            var rawResult = await ReadResults(path);
            return reader.Deserialize(rawResult.Results);
        }

        private async Task<JobCreationResults> ScheduleNewJob(string hiveQuery, string path, Guid jobIdentifier)
        {
            var jobParams = new HiveJobCreateParameters
            {
                StatusFolder = path,
                JobName = jobIdentifier.ToString(),
                Query = hiveQuery
            };

            var creationResult = await _jobClient.SubmitHiveJob(jobParams);
            return creationResult;
        }

        private async Task<JobResults> ReadResults(string path)
        {
            var resultPath = Path.Combine(path, _config.StandardOutputFileName);
            var errorPath = Path.Combine(path, _config.StandardErrorFileName);


            return new JobResults
            {
                Results = RetrieveJobResults(resultPath).Result,
                ErrorMessage = RetrieveJobResults(errorPath).Result
            };
        }

        private async Task<string> RetrieveJobResults(string resultPath)
        {
            var queryResult = await _hdfsClient.OpenFile(resultPath);
            using (var reader = new StreamReader(queryResult))
            {
                return reader.ReadToEnd();
            }
        }
    }

    public class JobResults
    {
        public string Results;
        public string ErrorMessage;
    }
}