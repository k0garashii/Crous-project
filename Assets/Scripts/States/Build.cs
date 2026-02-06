using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Build", menuName = "Scriptable Objects/State/Build")]
public class Build : State
{
    [Header("Build Settings")]
    public GameObject housePrefab;
    public GameObject buildPlaceholderPrefab;
    public float buildDistance = 1.2f;
    public float buildTime = 5f;
    public BuildProgressUI buildUI;

    public override void OnEnter(BuilderAgent agent) 
    {
        base.OnEnter(agent);
    }
    public override void HandleUpdate() 
    {
        HandleBuild();
    }
    public override void HandleCoroutine() { }
    public override void OnExit() { }

    void HandleBuild()
    {
        Building buildInfo = housePrefab.GetComponent<Building>();
        if (!buildInfo || !buildInfo.Build())
        {
            agent.actualState = StateManager.instance.GatherWood;
            return;
        }

        Vector3 targetPos = grid.CellToWorld(agent.targetX, agent.targetY);

        if (Vector3.Distance(agent.transform.position, targetPos) <= buildDistance)
        {
            Debug.Log(" Builder arrivé au point de construction");

            agent.aiPath.canMove = false;
            agent.StartCoroutine(BuildCoroutine(targetPos));
        }
    }

    void BuildHouse(Vector3 position)
    {
        GameObject house = Instantiate(housePrefab, position, Quaternion.identity);

        if (!house.GetComponent<House>())
            house.AddComponent<House>();

        grid.Occupy(agent.targetX, agent.targetY);

        agent.aiPath.canMove = true;
        agent.destinationSetter.target = null;

        Debug.Log(" Maison construite avec succès !");
    }

    IEnumerator BuildCoroutine(Vector3 position)
    {
        Debug.Log(" Début de la construction");

        GameObject placeholder = GameObject.Instantiate(
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
        agent.actualState = null;
    }
}
