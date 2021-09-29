public class Job
{
    public string Message { get; set; }

    public JobType Type {  get; set; }

    public int HowManySecondsWillJobTake {  get; set; }


    public bool ShouldFaillOnWorkerTwo { get; set; }

}
