using System.Collections.Generic;
using UnityEngine;

public class House : MonoBehaviour
{
    [Header("Settings")]
    public int maxOccupants = 4;

    [Header("Points")]
    public Transform ExitPoint;

    private List<BuilderAgent> occupants = new();

    public bool CanEnter()
    {
        return occupants.Count < maxOccupants;
    }

    public void AddOccupant(BuilderAgent agent)
    {
        if (!occupants.Contains(agent))
            occupants.Add(agent);
    }

    public void RemoveOccupant(BuilderAgent agent)
    {
        occupants.Remove(agent);
    }

    public void ShowInfo()
    {
        Debug.Log($"Maison : {occupants.Count}/{maxOccupants} occupants");
    }

    public Vector3 GetSafeExitPosition()
    {
        if (ExitPoint != null)
            return ExitPoint.position;

        return transform.position + transform.forward * 1.5f;
    }
    public void HandleClick(int mouseButton)
    {
        if (mouseButton == 0)
        {
            BuilderAgent builder = FindFirstObjectByType<BuilderAgent>();
            if (!builder)
                builder = occupants[0];
            builder.ToggleHouse(this);
        }
        else if (mouseButton == 1)
        {
            ShowInfo();
        }
    }
}
