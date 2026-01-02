using Pathfinding;
using UnityEngine;

public class test : MonoBehaviour
{
    AIDestinationSetter aiDestinationSetter;
    public Transform target;
    public GameObject ElementToInstantiate;
    private bool spawned = false;
    public void Update()
    {
        if (Vector3.Distance(transform.position, aiDestinationSetter.target.position) <= 1)
        {
            if (spawned == false)
            { 
                Debug.Log("Reached Target");
                Instantiate(ElementToInstantiate, transform.position, Quaternion.identity);
                spawned = true;
            }
        }
     
    }   
    public void Start()
    {
        aiDestinationSetter = GetComponent<AIDestinationSetter>();
        aiDestinationSetter.target = target;
    }

    public void SetTarget(Transform position)
    {
        aiDestinationSetter.target = position;

    }
}
