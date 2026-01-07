using UnityEngine;

public class Plan : MonoBehaviour
{
    public GameObject ground;
    public float cellSize = 1f;

    private int sizeX;
    private int sizeY;
    private Vector3 origin;

    void OnDrawGizmos()
    {
        if (ground == null) return;

        
        Vector3 scale = ground.transform.localScale;
        float width = 10f * scale.x;
        float height = 10f * scale.z;

        sizeX = Mathf.RoundToInt(width / cellSize);
        sizeY = Mathf.RoundToInt(height / cellSize);

        
        origin = ground.transform.position
               - new Vector3(width / 2f, 0, height / 2f);

        Gizmos.color = Color.green;

        
        for (int x = 0; x <= sizeX; x++)
        {
            Vector3 start = origin + new Vector3(x * cellSize, 0, 0);
            Vector3 end = start + new Vector3(0, 0, height);
            Gizmos.DrawLine(start, end);
        }

        
        for (int y = 0; y <= sizeY; y++)
        {
            Vector3 start = origin + new Vector3(0, 0, y * cellSize);
            Vector3 end = start + new Vector3(width, 0, 0);
            Gizmos.DrawLine(start, end);
        }
    }
}
    