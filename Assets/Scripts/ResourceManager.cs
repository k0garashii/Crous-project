using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public int wood;
    public int stone;
    public int clay;
    public int iron;
    public int gold;
    public int food;
    public int water;
    public int energy;
    public int reasearchPoints;
    public int happiness;
    public int population;

    [HideInInspector] public static ResourceManager Instance;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
