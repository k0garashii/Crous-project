using UnityEngine;

public class ZoneManager : MonoBehaviour
{
    public static ZoneManager instance;

    public GameObject woodZone;
    void Start()
    {
        instance = this;
    }
}
