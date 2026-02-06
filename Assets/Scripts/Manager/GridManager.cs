using UnityEngine;

public class GridManager : MonoBehaviour
{
    public Renderer ground;
    public float cellSize = 1f;

    private int sizeX, sizeY;
    private Vector3 origin;
    private bool[,] occupied;

    public static GridManager instance;

    void Awake()
    {
        instance = this;

        Bounds b = ground.bounds;

        sizeX = Mathf.FloorToInt(b.size.x / cellSize);
        sizeY = Mathf.FloorToInt(b.size.z / cellSize);

        origin = new Vector3(b.min.x, 0, b.min.z);
        occupied = new bool[sizeX, sizeY];
    }

    public bool WorldToCell(Vector3 world, out int x, out int y)
    {
        x = Mathf.FloorToInt((world.x - origin.x) / cellSize);
        y = Mathf.FloorToInt((world.z - origin.z) / cellSize);

        return x >= 0 && y >= 0 && x < sizeX && y < sizeY;
    }

    public Vector3 CellToWorld(int x, int y)
    {
        return origin + new Vector3(
            x * cellSize + cellSize / 2f,
            0,
            y * cellSize + cellSize / 2f
        );
    }

    public bool IsFree(int x, int y) => !occupied[x, y];
    public void Occupy(int x, int y) => occupied[x, y] = true;

    void OnDrawGizmos()
    {
        if (ground == null) return;

        Bounds b = ground.bounds;
        Gizmos.color = Color.green;

        for (float x = b.min.x; x <= b.max.x; x += cellSize)
            Gizmos.DrawLine(new Vector3(x, 0, b.min.z), new Vector3(x, 0, b.max.z));

        for (float z = b.min.z; z <= b.max.z; z += cellSize)
            Gizmos.DrawLine(new Vector3(b.min.x, 0, z), new Vector3(b.max.x, 0, z));
    }
}
