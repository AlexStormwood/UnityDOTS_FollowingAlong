using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

public class BasicExample : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        DoExample();
    }

    private void DoExample()
    {
        // create & set aside data as early as possible:
        NativeArray<float> resultArray = new NativeArray<float>(1, Allocator.TempJob);

        // instantiate the job
        //SimpleJob myJob = new SimpleJob();
        // initialize the job instance with data
        //myJob.a = 5;
        //myJob.result = new NativeArray<float>(1, Allocator.TempJob);

        // Do the above but using object initializer syntax:
        SimpleJob myJob = new SimpleJob
        {
            a = 5f,
            result = resultArray
        };
        AnotherJob secondJob = new AnotherJob
        {
            result = resultArray
        };
        // Allocator controls how long the job's data lasts. Most common options are:
        // Temp = 1 frame
        // TempJob = lifetime of Job or 4 frames, whichever comes first
        // Persistent = as long as you need

        // schedule the job instance into the CPU 
        JobHandle handle = myJob.Schedule();
        JobHandle secondHandle = secondJob.Schedule(handle); // requires handle to complete before running!

        // ...
        // other tasks to run in Main Thread in parallel with other jobs:
        // ...
        
        //handle.Complete(); // dependency of handles implies the first one already completes
        secondHandle.Complete();
        
        // job must complete before accessing job data, especially native container data!
        float resultValue = resultArray[0];
        Debug.Log($"Result was {resultValue}"); // gets value from native container, so we can access it
        Debug.Log($"myJob.a has a value of {myJob.a}"); // gets value from copy of job data, does not 
        resultArray.Dispose();
    }

    private struct SimpleJob : IJob
    {
        public float a;
        public NativeArray<float> result;
        public void Execute()
        {
            result[0] = a;
            a = 23f;
        }
    }

    private struct AnotherJob: IJob
    {
        public NativeArray<float> result;

        public void Execute()
        {
            result[0] = result[0] + 1;
        }
    }
}
