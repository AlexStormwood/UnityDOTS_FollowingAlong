using Unity.Entities;

[GenerateAuthoringComponent]
public struct WaveDataJobified : IComponentData
{
    public float amplitude;
    public float xOffset;
    public float yOffset;
}
