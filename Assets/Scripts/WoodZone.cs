using UnityEngine;
using System.Collections;

public class WoodZone : MonoBehaviour
{
    public int woodPerTick = 1;
    public float gatherInterval = 1f;

    private Coroutine gatheringRoutine;

    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<BuilderAgent>())
        {
            Debug.Log("🌲 Début récolte bois");
            if (gatheringRoutine == null)
                gatheringRoutine = StartCoroutine(GatherWoodRoutine());
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<BuilderAgent>())
        {
            Debug.Log("🚶 Sortie zone bois");
            if (gatheringRoutine != null)
            {
                StopCoroutine(gatheringRoutine);
                gatheringRoutine = null;
            }
        }
    }

    IEnumerator GatherWoodRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(gatherInterval);

            if (ResourceManager.Instance)
            {
                ResourceManager.Instance.wood += woodPerTick;
                Debug.Log($"🪵 +{woodPerTick} bois | Total = {ResourceManager.Instance.wood}");
            }
        }
    }
}