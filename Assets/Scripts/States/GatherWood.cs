using UnityEngine;

[CreateAssetMenu(fileName = "GatherWood", menuName = "Scriptable Objects/State/GatherWood")]
public class GatherWood : State
{
    [Header("Wood Gathering")]
    public int requiredWood = 5;

    public override void OnEnter(BuilderAgent agent)
    {
        base.OnEnter(agent);
        agent.destinationSetter.target.position = ZoneManager.instance.woodZone.transform.position;
    }
    public override void HandleUpdate()
    {
        HasEnoughWood();
    }
    public override void HandleCoroutine() { }
    public override void OnExit() { }

    private void HasEnoughWood()
    {
        if(ResourceManager.Instance.wood >= requiredWood)
        {
            agent.actualState = StateManager.instance.Build;
        }
    }
}
