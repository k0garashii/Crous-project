using UnityEngine;

public class MouseClickManager : MonoBehaviour
{
    public Camera cam;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                HouseInteraction house = hit.collider.GetComponentInParent<HouseInteraction>();
                if (house != null)
                {
                    house.HandleClick(Input.GetMouseButtonDown(0) ? 0 : 1);
                }
            }
        }
    }
}
