using Pathfinding;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject target;
    private AIDestinationSetter aiDestinationSetter;
    
    public void Start()
    {
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        SetTarget(target.transform);
    }

    public void SetTarget(Transform position)
    {
        aiDestinationSetter.target = position;
    }
}
