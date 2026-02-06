using Pathfinding;
using UnityEngine;
using System.Collections;

public class BuilderAgent : MonoBehaviour
{
    [Header("References")]
    public GridManager grid;

    [Header("Build Settings")]
    public GameObject housePrefab;
    public GameObject buildPlaceholderPrefab;
    public float buildDistance = 1.2f;
    public float buildTime = 5f;

    public BuildProgressUI buildUI;

    [Header("Wood Gathering")]
    public Transform woodZoneTarget;
    public int requiredWood = 5;

    private AIDestinationSetter destinationSetter;
    private AIPath aiPath;

    private Transform buildTarget;
    private int targetX;
    private int targetY;

    private bool hasTarget = false;
    private bool hasBuilt = false;
    private bool isBuilding = false;
    private bool gatheringWood = false;

    private Vector3 pendingBuildPosition;

    // ===== House =====
    private House currentHouse;
    private bool isInside = false;

    void Awake()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();

        Debug.Log("👷 Builder initialisé");
    }

    void Update()
    {
        // 🔍 DEBUG état global
        Debug.Log(
            $"[Builder] Etat | Bois={GetWood()} | gathering={gatheringWood} | building={isBuilding}"
        );

        // Si on récolte du bois
        if (gatheringWood)
        {
            // Dès que le ResourceManager a assez de bois → on repart
            if (HasEnoughWood())
            {
                Debug.Log(" Bois suffisant détecté → retour vers la construction");

                gatheringWood = false;
                destinationSetter.target = null;

                MoveToBuildPosition(pendingBuildPosition);
            }

            return;
        }

        HandleBuild();
    }

    // ===================== BUILD =====================

    public void SetBuildCell(int x, int y)
    {
        if (hasBuilt || isBuilding || grid == null)
        {
            Debug.Log(" Impossible de définir une cellule de construction");
            return;
        }

        targetX = x;
        targetY = y;

        Vector3 worldPos = grid.CellToWorld(x, y);
        pendingBuildPosition = worldPos;

        Debug.Log($" Demande de construction en {x},{y}");

        // Pas assez de bois → aller en chercher
        if (!HasEnoughWood())
        {
            Debug.Log(" Bois insuffisant → départ vers la WoodZone");
            GoGatherWood();
            return;
        }

        MoveToBuildPosition(worldPos);
    }

    void MoveToBuildPosition(Vector3 position)
    {
        if (buildTarget == null)
        {
            GameObject t = new GameObject("BuildTarget");
            buildTarget = t.transform;
        }

        buildTarget.position = position;

        destinationSetter.target = buildTarget;
        aiPath.canMove = true;
        aiPath.SearchPath();

        hasTarget = true;

        Debug.Log(" Builder se déplace vers la position de construction");
    }

    void GoGatherWood()
    {
        if (!woodZoneTarget)
        {
            Debug.LogError(" WoodZoneTarget NON assignée dans le BuilderAgent");
            return;
        }

        gatheringWood = true;

        destinationSetter.target = woodZoneTarget;
        aiPath.canMove = true;
        aiPath.SearchPath();

        Debug.Log(" Builder en route vers la WoodZone");
    }

    bool HasEnoughWood()
    {
        return ResourceManager.Instance != null &&
               ResourceManager.Instance.wood >= requiredWood;
    }

    int GetWood()
    {
        if (ResourceManager.Instance == null)
            return -1;

        return ResourceManager.Instance.wood;
    }

    void HandleBuild()
    {
        if (!hasTarget || hasBuilt || isBuilding)
            return;

        Vector3 targetPos = grid.CellToWorld(targetX, targetY);

        if (Vector3.Distance(transform.position, targetPos) <= buildDistance)
        {
            Debug.Log(" Builder arrivé au point de construction");

            aiPath.canMove = false;
            StartCoroutine(BuildCoroutine(targetPos));
        }
    }

    IEnumerator BuildCoroutine(Vector3 position)
    {
        isBuilding = true;

        Debug.Log(" Début de la construction");

        GameObject placeholder = Instantiate(
            buildPlaceholderPrefab,
            position,
            Quaternion.identity
        );

        buildUI = placeholder.GetComponentInChildren<BuildProgressUI>(true);

        float timer = 0f;

        if (buildUI != null)
            buildUI.Init(buildTime);

        while (timer < buildTime)
        {
            timer += Time.deltaTime;

            if (buildUI != null)
                buildUI.SetProgress(timer);

            yield return null;
        }

        Destroy(placeholder);
        BuildHouse(position);

        isBuilding = false;
    }

    void BuildHouse(Vector3 position)
    {
        Debug.Log(" Tentative de construction de la maison");

        Building buildInfo = housePrefab.GetComponent<Building>();
        if (!buildInfo || !buildInfo.Build())
        {
            Debug.LogError(" Construction annulée : ressources insuffisantes");
            return;
        }

        GameObject house = Instantiate(housePrefab, position, Quaternion.identity);

        if (!house.GetComponent<House>())
            house.AddComponent<House>();

        grid.Occupy(targetX, targetY);

        hasBuilt = true;
        hasTarget = false;

        aiPath.canMove = true;
        destinationSetter.target = null;

        Debug.Log(" Maison construite avec succès !");
    }

    // ===================== HOUSE =====================

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
