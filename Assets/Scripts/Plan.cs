using UnityEngine;

public class Plan : MonoBehaviour
{
    public Renderer groundRenderer;
    public float cellSize = 1f;

    private int sizeX;
    private int sizeY;
    private Vector3 origin;
    private bool[,] occupied;
    private bool isReady = false;

    void Awake()
    {
        if (groundRenderer == null)
        {
            Debug.LogError("Plan : GroundRenderer manquant");
            return;
        }

        if (cellSize <= 0)
        {
            Debug.LogError("Plan : CellSize doit être > 0");
            cellSize = 1f;
        }

        InitGrid();
    }

    void InitGrid()
    {
        Bounds bounds = groundRenderer.bounds;

        sizeX = Mathf.Max(1, Mathf.FloorToInt(bounds.size.x / cellSize));
        sizeY = Mathf.Max(1, Mathf.FloorToInt(bounds.size.z / cellSize));

        origin = new Vector3(bounds.min.x, 0, bounds.min.z);
        occupied = new bool[sizeX, sizeY];

        isReady = true;
    }

    private void OnDrawGizmos()
    {
        if (!isReady || groundRenderer == null) return;

        Gizmos.color = Color.green;

        for (int x = 0; x <= sizeX; x++)
        {
            Vector3 start = origin + new Vector3(x * cellSize, 0, 0);
            Vector3 end = start + new Vector3(0, 0, sizeY * cellSize);
            Gizmos.DrawLine(start, end);
        }

        for (int y = 0; y <= sizeY; y++)
        {
            Vector3 start = origin + new Vector3(0, 0, y * cellSize);
            Vector3 end = start + new Vector3(sizeX * cellSize, 0, 0);
            Gizmos.DrawLine(start, end);
        }
    }

    public bool IsCellFree(int x, int y)
    {
        if (!isReady) return false;
        return !occupied[x, y];
    }

    public void OccupyCell(int x, int y)
    {
        if (!isReady) return;
        occupied[x, y] = true;
    }

    public Vector3 GetCellCenter(int x, int y)
    {
        return origin + new Vector3(
            x * cellSize + cellSize / 2f,
            0,
            y * cellSize + cellSize / 2f
        );
    }

    public int SizeX => sizeX;
    public int SizeY => sizeY;
    public bool Ready => isReady;
}
