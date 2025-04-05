using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Основные настройки")]
    [SerializeField] private int roomCount = 5;
    [SerializeField] private Vector2Int minRoomSize = new Vector2Int(4, 4);
    [SerializeField] private Vector2Int maxRoomSize = new Vector2Int(10, 10);
    [SerializeField] private int dungeonWidth = 100;
    [SerializeField] private int dungeonHeight = 100;

    [Header("Настройки коридоров")]
    [SerializeField][Range(1, 5)] private int corridorWidth = 1;
    [SerializeField][Range(0.1f, 2f)] private float corridorLengthFactor = 1f;
    [SerializeField] private bool windingCorridors = false;
    [SerializeField][Range(0f, 1f)] private float windingIntensity = 0.3f;

    [Header("Тайлы")]
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase corridorTile;

    [Header("Компоненты")]
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap wallMap;

    private List<Room> rooms = new List<Room>();
    private System.Random random;

    private class Room
    {
        public RectInt bounds;
        public Vector2Int center;

        public Room(RectInt bounds)
        {
            this.bounds = bounds;
            this.center = new Vector2Int(bounds.x + bounds.width / 2, bounds.y + bounds.height / 2);
        }

        public bool Intersects(Room other, int padding = 1)
        {
            RectInt thisRect = new RectInt(bounds.x - padding, bounds.y - padding, bounds.width + padding * 2, bounds.height + padding * 2);
            RectInt otherRect = new RectInt(other.bounds.x - padding, other.bounds.y - padding, other.bounds.width + padding * 2, other.bounds.height + padding * 2);
            return thisRect.x < otherRect.x + otherRect.width && thisRect.x + thisRect.width > otherRect.x && thisRect.y < otherRect.y + otherRect.height && thisRect.y + thisRect.height > otherRect.y;
        }
    }

    void Start()
    {
        random = new System.Random();
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        ClearMaps();
        GenerateRooms();
        ConnectRooms();
        CreateWalls();
    }

    void ClearMaps()
    {
        floorMap.ClearAllTiles();
        wallMap.ClearAllTiles();
        rooms.Clear();
    }

    void GenerateRooms()
    {
        for (int i = 0; i < roomCount; i++)
        {
            int attempts = 0;
            bool roomPlaced = false;

            while (!roomPlaced && attempts < 100)
            {
                attempts++;
                int width = random.Next(minRoomSize.x, maxRoomSize.x + 1);
                int height = random.Next(minRoomSize.y, maxRoomSize.y + 1);
                int x = random.Next(0, dungeonWidth - width);
                int y = random.Next(0, dungeonHeight - height);

                RectInt newRoomBounds = new RectInt(x, y, width, height);
                Room newRoom = new Room(newRoomBounds);

                if (!rooms.Any(room => room.Intersects(newRoom)))
                {
                    rooms.Add(newRoom);
                    roomPlaced = true;
                    DrawRoom(newRoom);
                }
            }
        }
    }

    void DrawRoom(Room room)
    {
        for (int x = room.bounds.x; x < room.bounds.x + room.bounds.width; x++)
        {
            for (int y = room.bounds.y; y < room.bounds.y + room.bounds.height; y++)
            {
                floorMap.SetTile(new Vector3Int(x, y, 0), floorTile);
            }
        }
    }

    void ConnectRooms()
    {
        var sortedRooms = rooms.OrderBy(room => room.center.x).ToList();

        for (int i = 0; i < sortedRooms.Count - 1; i++)
        {
            ConnectTwoRooms(sortedRooms[i], sortedRooms[i + 1]);
        }
    }

    void ConnectTwoRooms(Room a, Room b)
    {
        Vector2Int start = a.center;
        Vector2Int end = b.center;
        Vector2Int horizontalEnd = new Vector2Int(end.x, start.y);
        Vector2Int verticalStart = horizontalEnd;

        if (corridorLengthFactor != 1f)
        {
            int baseLength = end.x - start.x;
            int modifiedLength = Mathf.RoundToInt(baseLength * corridorLengthFactor);
            horizontalEnd.x = start.x + modifiedLength;
            verticalStart = horizontalEnd;
        }

        if (windingCorridors)
        {
            int windingOffset = Mathf.RoundToInt((end.y - start.y) * windingIntensity * (float)(random.NextDouble() * 2 - 1));
            horizontalEnd.y += windingOffset;
            verticalStart.y += windingOffset;
        }

        DrawCorridorLine(start, horizontalEnd);
        DrawCorridorLine(verticalStart, end);
    }

    void DrawCorridorLine(Vector2Int start, Vector2Int end)
    {
        int dx = Mathf.Abs(end.x - start.x);
        int dy = Mathf.Abs(end.y - start.y);
        int sx = start.x < end.x ? 1 : -1;
        int sy = start.y < end.y ? 1 : -1;
        int err = dx - dy;
        int halfWidth = corridorWidth / 2;
        int widthOffset = -(corridorWidth - 1) / 2;

        while (true)
        {
            for (int w = 0; w < corridorWidth; w++)
            {
                int wx = dx > dy ? start.x : start.x + widthOffset + w;
                int wy = dx > dy ? start.y + widthOffset + w : start.y;
                Vector3Int pos = new Vector3Int(wx, wy, 0);
                if (!floorMap.HasTile(pos))
                {
                    floorMap.SetTile(pos, corridorTile ?? floorTile);
                }
            }

            if (start.x == end.x && start.y == end.y) break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                start.x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                start.y += sy;
            }
        }
    }

    void CreateWalls()
    {
        HashSet<Vector3Int> floorPositions = new HashSet<Vector3Int>();

        foreach (var pos in floorMap.cellBounds.allPositionsWithin)
        {
            if (floorMap.HasTile(pos))
            {
                floorPositions.Add(pos);
            }
        }

        foreach (var floorPos in floorPositions)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    Vector3Int neighborPos = new Vector3Int(floorPos.x + x, floorPos.y + y, floorPos.z);
                    if (!floorPositions.Contains(neighborPos))
                    {
                        wallMap.SetTile(neighborPos, wallTile);
                    }
                }
            }
        }
    }
}