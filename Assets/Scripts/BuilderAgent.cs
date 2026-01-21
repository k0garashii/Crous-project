using Pathfinding;
using UnityEngine;

public class BuilderAgent : MonoBehaviour
{
    [Header("References")]
    public GridManager grid;
    public GameObject housePrefab;

    [Header("Build Settings")]
    public float buildDistance = 1.2f;

    private AIDestinationSetter destinationSetter;
    private AIPath aiPath;

    private Transform buildTarget;
    private int targetX;
    private int targetY;
    private bool hasTarget = false;
    private bool hasBuilt = false;

    private House currentHouse;
    private bool isInside = false;

    void Awake()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
    }

    void Update()
    {
        HandleBuild();
    }

    public void SetBuildCell(int x, int y)
    {
        if (hasBuilt || grid == null) return;

        targetX = x;
        targetY = y;

        Vector3 worldPos = grid.CellToWorld(x, y);

        if (buildTarget == null)
        {
            GameObject t = new GameObject("BuildTarget");
            buildTarget = t.transform;
        }

        buildTarget.position = worldPos;

        destinationSetter.target = buildTarget;
        aiPath.SearchPath();

        hasTarget = true;
    }

    void HandleBuild()
    {
        if (!hasTarget || hasBuilt) return;

        Vector3 targetPos = grid.CellToWorld(targetX, targetY);

        if (Vector3.Distance(transform.position, targetPos) <= buildDistance)
        {
            BuildHouse(targetPos);
            aiPath.canMove = false;
        }
    }

    void BuildHouse(Vector3 position)
    {
        GameObject house = Instantiate(housePrefab, position, Quaternion.identity);

        if (!house.GetComponent<House>())
            house.AddComponent<House>();

        grid.Occupy(targetX, targetY);

        hasBuilt = true;
        hasTarget = false;

        Debug.Log("Maison construite");
    }

    public void ToggleHouse(House house)
    {
        if (house == null) return;

        if (!isInside)
            EnterHouse(house);
        else
            ExitHouse();
    }

    //Utilitaries 
    void EnterHouse(House house)
    {
        if (!house.CanEnter()) return;

        currentHouse = house;
        house.AddOccupant(this);

        HideBuilder();
        isInside = true;

        Debug.Log("Builder entré dans la maison");
    }

    void ExitHouse()
    {
        if (currentHouse == null) return;

        Vector3 exitPos = currentHouse.GetSafeExitPosition();
        transform.position = exitPos;

        currentHouse.RemoveOccupant(this);

        isInside = false;
        currentHouse = null;
        aiPath.canMove = true;
        destinationSetter.target = null;
        aiPath.SearchPath();
        ShowBuilder();

        Debug.Log("Builder sorti de la maison");
    }

    void HideBuilder()
    {
        transform.gameObject.SetActive(false);
    }

    void ShowBuilder()
    {
        transform.gameObject.SetActive(true);
    }
}
