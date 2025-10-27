using Pathfinding;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    AIDestinationSetter aiDestinationSetter;
    public void Start()
    {
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
    }

    public void SetTarget(Transform position)
    {
        aiDestinationSetter.target = position;
    }
}
