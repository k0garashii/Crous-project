using UnityEngine;

public class HouseInteraction : MonoBehaviour
{
    private House house;

    void Awake()
    {
        house = GetComponent<House>();
    }

    public void HandleClick(int mouseButton)
    {
        BuilderAgent builder = FindFirstObjectByType<BuilderAgent>();
        if (builder == null) return;

        if (mouseButton == 0)
        {
            builder.ToggleHouse(house);
        }
        else if (mouseButton == 1)
        {
            house.ShowInfo();
        }
    }
}
