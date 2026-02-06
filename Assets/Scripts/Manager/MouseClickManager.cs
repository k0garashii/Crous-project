using UnityEngine;

public class MouseClickManager : MonoBehaviour
{
    public Camera cam;
    public GridManager grid;
    public BuilderAgent builder;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                House house = hit.collider.GetComponent<House>();
                if (house != null)
                {
                    house.HandleClick(Input.GetMouseButtonDown(0) ? 0 : 1);
                }
            }
        }
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (grid.WorldToCell(hit.point, out int x, out int y))
                {
                    if (grid.IsFree(x, y))
                    {
                        builder.actualState.SetDestination(x, y);
                    }
                }
            }
        }
    }
}
