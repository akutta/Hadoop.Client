using Newtonsoft.Json;

namespace Hadoop.Client.Jobs.Models
{
    public class JobDetailsResponse
    {
        [JsonProperty("status")]
        public JobStatus Status;

        [JsonProperty("percentComplete")]
        public string PercentComplete;

    }

    public class JobStatus
    {
        [JsonProperty("runState")]
        public int RunState;

        [JsonProperty("trackingUrl")] 
        public string TrackingUrl;

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
