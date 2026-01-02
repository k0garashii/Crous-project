using UnityEngine;

public class Building : MonoBehaviour
{
    public ScriptableObject buildingData;
    public int sizeX;
    public int sizeY;
    private ResourceManager resourceManager;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool Build()
    {
        if (buildingData is SO_Resources data)
        {
            if (resourceManager.wood >= data.wood &&
               resourceManager.stone >= data.stone &&
               resourceManager.clay >= data.clay &&
               resourceManager.iron >= data.iron &&
               resourceManager.gold >= data.gold &&
               resourceManager.food >= data.food &&
               resourceManager.water >= data.water &&
               resourceManager.energy >= data.energy)
            {
                resourceManager.wood -= data.wood;
                resourceManager.stone -= data.stone;
                resourceManager.clay -= data.clay;
                resourceManager.iron -= data.iron;
                resourceManager.gold -= data.gold;
                resourceManager.food -= data.food;
                resourceManager.water -= data.water;
                resourceManager.energy -= data.energy;
                Debug.Log($"{data} built successfully! {data.wood} wood restant");
                return true;
            }
            else
            {
                Debug.Log("Not enough resources to build!");
                return false;
            }
        }
        Debug.LogError("Invalid building data!");
        return false;
    }
}
