using Pathfinding;
using UnityEngine;

public class test : MonoBehaviour
{
    public Plan plan;
    public GameObject housePrefab;
    public float buildDistance = 1f;

    private AIDestinationSetter aiDestinationSetter;
    private bool hasBuilt = false;
    private Transform buildTarget;

    private int targetX;
    private int targetY;

    void Start()
    {
        aiDestinationSetter = GetComponent<AIDestinationSetter>();

        if (plan == null)
            plan = Object.FindFirstObjectByType<Plan>();

        if (plan == null || !plan.Ready)
        {
            Debug.LogError("Plan non prêt");
            enabled = false;
            return;
        }

        ChooseBuildCell();
    }

    void Update()
    {
        if (hasBuilt || buildTarget == null) return;

        float distance = Vector3.Distance(transform.position, buildTarget.position);

        if (distance <= buildDistance)
        {
            BuildHouse();
        }
    }

    void ChooseBuildCell()
    {
        for (int x = 0; x < plan.SizeX; x++)
        {
            for (int y = 0; y < plan.SizeY; y++)
            {
                if (plan.IsCellFree(x, y))
                {
                    targetX = x;
                    targetY = y;

                    Vector3 pos = plan.GetCellCenter(x, y);
                    buildTarget = CreateTarget(pos);
                    aiDestinationSetter.target = buildTarget;

                    return;
                }
            }
        }

        Debug.LogError("Aucune case libre trouvée");
    }

    void BuildHouse()
    {
        Instantiate(housePrefab, buildTarget.position, Quaternion.identity);
        plan.OccupyCell(targetX, targetY);
        hasBuilt = true;

        Debug.Log("Maison construite");
    }

    Transform CreateTarget(Vector3 position)
    {
        GameObject t = new GameObject("BuildTarget");
        t.transform.position = position;
        return t.transform;
    }
}
