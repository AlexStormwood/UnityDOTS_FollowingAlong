using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;

// JobComponentSystem is kinda old school
// SystemBase is newer & better for handling dependencies!
public class WaveSystemJobified : SystemBase
{
    protected override void OnUpdate()
    {
        // Main thread
        float elapsedTime = (float)Time.ElapsedTime;
        // Time.ElapsedTime is a ref type, can't be used in jobified blocks

        // SystemBase allows implicit jobs -- so if this Entities.ForEach block was copypasted,
        // it would assume that the first Entities.ForEach block must finish before the second block can start
        // Kinda turning this into 'interpreted code' as dependencies are detected from top to bottom!
        Entities.ForEach((ref Translation trans, in MoveSpeedDataJobified moveSpeed, in WaveDataJobified waveData) =>
        {
            // ref = read & write, full reference to data
            // in = read only reference, no writing
            // out = write only reference, no reading unless you save the out as another variable

            float zPos = waveData.amplitude * math.sin(elapsedTime * moveSpeed.Value + trans.Value.x * waveData.xOffset + trans.Value.y * waveData.yOffset);
            trans.Value = new float3(trans.Value.x, trans.Value.y, zPos);
        }).ScheduleParallel();
        // Schedule for threaded tasks
        // ScheduleParallel for parallel threaded tasks
        // Run for main-thread-only tasks


    }
}




// Using JobComponentSystem
//public class WaveSystemJobified : JobComponentSystem
//{
//    protected override JobHandle OnUpdate(JobHandle inputDeps)
//    {
//        // Main thread
//        float elapsedTime = (float)Time.ElapsedTime;
//        // Time.ElapsedTime is a ref type, can't be used in jobified blocks

//        // JobHandle block is suitable for worker threads
//        JobHandle jobHandle = Entities.ForEach((ref Translation trans, in MoveSpeedDataJobified moveSpeed, in WaveDataJobified waveData) =>
//        {
//            // ref = read & write, full reference to data
//            // in = read only reference, no writing
//            // out = write only reference, no reading unless you save the out as another variable

//            float zPos = waveData.amplitude * math.sin(elapsedTime * moveSpeed.Value + trans.Value.x * waveData.xOffset + trans.Value.y * waveData.yOffset);
//            trans.Value = new float3(trans.Value.x, trans.Value.y, zPos);
//        }).Schedule(inputDeps);
//        // schedule tells Unity that the block should be autoassigned to threads 

//        return jobHandle;

//        // Above code is jobified & suitable for worker or non-main threads
//        // To make a job suitable only for the main thread, you must:
//        // - get rid of the "JobHandle jobHandle = " part
//        // - replace ".Schedule(inputDeps)" with ".Run()"
//        // - replace "return jobHandle" with "returnDefault"
//        // - add attribute "[AlwaysSynchronizeSystem]" above class definition
//        // Main threads are best for UI, user inputs, and other crucial info
//        // Worker threads are best for large processing tasks that can be broken into small steps
//        // such as terrain generation or entity movement
//    }
//}
