using UnityEngine;

public class StateManager : MonoBehaviour
{
    public State Build;
    public State GatherWood;

    public static StateManager instance;

    public void Awake()
    {
        instance = this;
    }
}
