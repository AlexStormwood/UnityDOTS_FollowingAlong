using Unity.Entities;
using UnityEngine;
using Unity.Transforms;
using Unity.Rendering;
using Unity.Mathematics;

public class SpawnerJobified : MonoBehaviour
{
    public Mesh unitMesh;
    public Material unitMaterial;

    public GameObject prefabToSpawn;
    private Entity entityPrefab;
    private World defWorldRef;
    private EntityManager emRef;

    public int xSize = 10;
    public int ySize = 10;
    [Range(0.1f, 10f)]
    public float spacing = 1;

    private void Awake()
    {
        defWorldRef = World.DefaultGameObjectInjectionWorld;
        emRef = defWorldRef.EntityManager;
    }

    private void Start()
    {
        // Spawn a pure ECS entity:
        // MakeEntity();

        // Set up conversion for Hybrid ECS:
        GameObjectConversionSettings settings = GameObjectConversionSettings.FromWorld(defWorldRef, null);
        // Convert gameobject into entity:
        entityPrefab = GameObjectConversionUtility.ConvertGameObjectHierarchy(prefabToSpawn, settings);

        //InstantiateEntity(new float3(4, 0, 4));
        InstantiateEntityGrid(xSize, ySize, spacing);
    }

    private void InstantiateEntity(float3 position)
    {
        // Instantiate & save ref for further editing:
        Entity newEntity = emRef.Instantiate(entityPrefab);
        // Set position of the new instance -- world 0,0,0 is default!
        emRef.SetComponentData(newEntity, new Translation
        {
            Value = position
        });
    }

    private void InstantiateEntityGrid(int dimX, int dimY, float spacing = 1f)
    {
        for (int i = 0; i < dimX; i++)
        {
            for (int j = 0; j < dimY; j++)
            {
                InstantiateEntity(new float3(i * spacing, j * spacing, 0f));
            }
        }
    }

    private void MakeEntity()
    {
        EntityManager emRef = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityArchetype archetype = emRef.CreateArchetype(
                // Recreate standard Transform data
                typeof(Translation), // transform.position
                typeof(Rotation), // transform.rotation
                // three below all needed for HybridRenderer to draw the object:
                typeof(RenderMesh), // meshfilter & meshrenderer combined
                typeof(RenderBounds), // transform.scale or bounding box of mesh
                typeof(LocalToWorld) // local -> global space converter
            );
        Entity entityInstance = emRef.CreateEntity(archetype);
        emRef.AddComponentData(entityInstance, new Translation 
        {
            // Translation from Unity.Transforms
            // float3 from Unity.Mathematics
            Value = new float3(2f, 0f, 4f)
        });
        emRef.AddSharedComponentData(entityInstance, new RenderMesh
        {
            // From Unity.Rendering, the HybridRenderer
            mesh = unitMesh,
            material = unitMaterial
        });
    }



}
