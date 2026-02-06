using Pathfinding;
using UnityEngine;

public class BuilderAgent : MonoBehaviour
{
    [Header("References")]
    public State actualState;
    private State lastState;

    public AIDestinationSetter destinationSetter { get; set; }
    public AIPath aiPath { get; set; }
    public int targetX { get; set; }
    public int targetY { get; set; }

    // ===== House =====
    private House currentHouse;
    private bool isInside = false;

    void Awake()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
        actualState.OnEnter(this);
    }

    void Update() 
    {
        if (lastState != actualState)
        {
            actualState.OnEnter(this);
            lastState = actualState;
        }
        if(actualState)
            actualState.HandleUpdate();
    }

    public void ToggleHouse(House house)
    {
        if (!house) return;

        if (!isInside)
            EnterHouse(house);
        else
            ExitHouse();
    }

    void EnterHouse(House house)
    {
        if (!house.CanEnter()) return;

        currentHouse = house;
        house.AddOccupant(this);

        gameObject.SetActive(false);
        isInside = true;

        Debug.Log(" Builder entré dans la maison");
    }

    void ExitHouse()
    {
        if (currentHouse == null) return;

        Vector3 exitPos = currentHouse.GetSafeExitPosition();
        transform.position = exitPos + Vector3.up * 0.1f;

        currentHouse.RemoveOccupant(this);

        gameObject.SetActive(true);
        isInside = false;
        currentHouse = null;

        destinationSetter.target = null;
        aiPath.canMove = true;
        aiPath.SearchPath();

        Debug.Log(" Builder sorti de la maison");
    }
}
