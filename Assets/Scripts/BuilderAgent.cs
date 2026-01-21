using Pathfinding;
using UnityEngine;
using System.Collections;

public class BuilderAgent : MonoBehaviour
{
    [Header("References")]
    public GridManager grid;
    public GameObject housePrefab;
    public GameObject buildPlaceholderPrefab;
    public BuildProgressUI buildUI;

    [Header("Build Settings")]
    public float buildDistance = 1.2f;
    public float buildTime = 5f;

    private AIDestinationSetter destinationSetter;
    private AIPath aiPath;

    private Transform buildTarget;
    private int targetX;
    private int targetY;

    private bool hasTarget = false;
    private bool hasBuilt = false;
    private bool isBuilding = false;

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

    // ===================== BUILD =====================

    public void SetBuildCell(int x, int y)
    {
        if (hasBuilt || isBuilding || grid == null) return;

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
        aiPath.canMove = true;
        aiPath.SearchPath();

        hasTarget = true;
    }

    void HandleBuild()
    {
        if (!hasTarget || hasBuilt || isBuilding) return;

        Vector3 targetPos = grid.CellToWorld(targetX, targetY);

        if (Vector3.Distance(transform.position, targetPos) <= buildDistance)
        {
            aiPath.canMove = false;
            StartCoroutine(BuildCoroutine(targetPos));
        }
    }

    IEnumerator BuildCoroutine(Vector3 position)
    {
        isBuilding = true;

        GameObject placeholder = Instantiate(
            buildPlaceholderPrefab,
            position,
            Quaternion.identity
        );

        float timer = 0f;

        if (buildUI != null)
        {
            buildUI.gameObject.SetActive(true);
            buildUI.Init(placeholder.transform, buildTime);
        }

        while (timer < buildTime)
        {
            timer += Time.deltaTime;

            if (buildUI != null)
                buildUI.SetProgress(timer);

            yield return null;
        }

        if (buildUI != null)
            buildUI.gameObject.SetActive(false);

        Destroy(placeholder);

        BuildHouse(position);

        isBuilding = false;
    }

    void BuildHouse(Vector3 position)
    {
        GameObject house = Instantiate(housePrefab, position, Quaternion.identity);

        if (!house.GetComponent<House>())
            house.AddComponent<House>();

        grid.Occupy(targetX, targetY);

        hasBuilt = true;
        hasTarget = false;

        aiPath.canMove = true;

        Debug.Log("Maison construite");
    }

    // ===================== House =====================

    public void ToggleHouse(House house)
    {
        if (house == null) return;

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

        Debug.Log("Builder entré dans la maison");
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

        Debug.Log("Builder sorti de la maison");
    }
}
