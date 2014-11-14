using System.Threading.Tasks;
using Hadoop.Client.Jobs.Models;

namespace Hadoop.Client.Jobs
{
    public interface IHadoopJobClient
    {
        Task<JobList> ListJobs();

        Task<JobDetailsResponse> GetJob(string jobId);

        Task<JobCreationResults> SubmitMapReduceJob(MapReduceJobCreateParameters details);

        Task<JobCreationResults> SubmitHiveJob(HiveJobCreateParameters details);

        Task<JobCreationResults> SubmitPigJob(PigJobCreateParameters pigJobCreateParameters);

        Task<JobCreationResults> SubmitSqoopJob(SqoopJobCreateParameters sqoopJobCreateParameters);

        Task<JobCreationResults> SubmitStreamingJob(StreamingMapReduceJobCreateParameters pigJobCreateParameters);

        Task<JobDetailsResponse> StopJob(string jobId);
    }
}
