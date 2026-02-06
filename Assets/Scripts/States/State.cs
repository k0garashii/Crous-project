using System.Collections;
using Pathfinding;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public abstract class State : ScriptableObject
{
    protected BuilderAgent agent;
    protected GridManager grid;

    public virtual void OnEnter(BuilderAgent agent) 
    {
        Initialize(agent);
    } // Appelé au début de l'état
    public virtual void HandleUpdate() { }
    public virtual void HandleCoroutine() { }
    public virtual void OnExit() { }  // Appelé à la fin

    public void SetDestination(int x, int y)
    {
        agent.targetX = x;
        agent.targetY = y;

        Vector3 worldPos = grid.CellToWorld(x, y);

        agent.destinationSetter.target.position = worldPos;
        agent.aiPath.canMove = true;
        agent.aiPath.SearchPath();

        Debug.Log(" Builder se déplace vers la destination");
    }

    private void Initialize(BuilderAgent agent)
    {
        this.agent = agent;
        grid = GridManager.instance;
    }

}