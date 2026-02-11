using Pathfinding;
using UnityEngine;

public class BuilderAgent : MonoBehaviour
{
    [Header("References")]
    public State actualState;
    private State lastState;

    public LLM_Manager llmManager;

    public string currentObjective;

    public AIDestinationSetter destinationSetter { get; private set; }
    public AIPath aiPath { get; private set; }

    public int targetX { get; set; }
    public int targetY { get; set; }

    void Awake()
    {
        destinationSetter = GetComponent<AIDestinationSetter>();
        aiPath = GetComponent<AIPath>();
    }

    void Update()
    {
        if (actualState != lastState)
        {
            actualState?.OnEnter(this);
            lastState = actualState;
        }

        actualState?.HandleUpdate();
    }
}
