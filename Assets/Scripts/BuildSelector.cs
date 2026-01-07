using UnityEngine;

public class BuildSelector : MonoBehaviour
{
    public Camera cam;
    public GridManager grid;
    public BuilderAgent builder;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (grid.WorldToCell(hit.point, out int x, out int y))
                {
                    if (grid.IsFree(x, y))
                    {
                        builder.SetBuildCell(x, y);
                    }
                }
            }
        }
    }
}
