using Newtonsoft.Json;

namespace Hadoop.Client.Jobs.Models
{
    public class JobDetailsResponse
    {
        [JsonProperty("status")]
        public JobStatus Status;

        [JsonProperty("percentComplete")]
        public string PercentComplete;

        [JsonProperty("userargs")]
        public UserArguments UserArgs;

    }

    public class UserArguments
    {
        [JsonProperty("statusdir")]
        public string StatusDirectory;

        [JsonProperty("files")]
        public string Files;


        [JsonProperty("file")]
        public string File;
    }

    public class JobStatus
    {
        [JsonProperty("runState")]
        public int RunState;

        [JsonProperty("trackingUrl")] 
        public string TrackingUrl;

        [JsonProperty("setupProgress")]
        public double SetupProgress;

        [JsonProperty("mapProgress")]
        public double MapProgress;
        
        [JsonProperty("reduceProgress")]
        public double ReduceProgress;

        [JsonProperty("cleanupProgress")]
        public double CleanUpProgress;

        [JsonProperty("jobComplete")] 
        public bool JobComplete;

        [JsonProperty("jobFile")]
        public string JobFile;

        [JsonProperty("startTime")]
        public long StartTime;

        [JsonProperty("finishTime")] 
        public long FinishTime;
    }}
