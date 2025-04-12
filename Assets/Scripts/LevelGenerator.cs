using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;

public class DungeonGenerator : MonoBehaviour
{
    [SerializeField] private int roomCount = 5;
    [SerializeField] private Vector2Int minRoomSize = new Vector2Int(4, 4);
    [SerializeField] private Vector2Int maxRoomSize = new Vector2Int(8, 8);
    [SerializeField] private int dungeonSize = 50;

    [SerializeField] private int corridorWidth = 1;

    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase corridorTile;

    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap wallMap;

    [SerializeField] private GameObject playerPrefab;

    private List<Room> rooms = new List<Room>();
    private System.Random random = new System.Random();

    private class Room
    {
        public RectInt bounds;
        public Vector2Int Center => new Vector2Int(bounds.x + bounds.width / 2, bounds.y + bounds.height / 2);
        public List<Room> neighbors = new List<Room>();

        public Room(RectInt bounds)
        {
            this.bounds = bounds;
        }
    }

    void Start()
    {
        GenerateDungeon();
        SpawnPlayerInRandomRoom();
    }

    public void GenerateDungeon()
    {
        ClearMaps();
        GenerateRooms();
        ConnectAllRooms();
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
            bool placed = false;

            while (!placed && attempts < 100)
            {
                attempts++;
                int width = random.Next(minRoomSize.x, maxRoomSize.x + 1);
                int height = random.Next(minRoomSize.y, maxRoomSize.y + 1);
                int x = random.Next(0, dungeonSize - width);
                int y = random.Next(0, dungeonSize - height);

                RectInt bounds = new RectInt(x, y, width, height);
                if (!rooms.Any(r => Overlaps(r.bounds, bounds, 2)))
                {
                    rooms.Add(new Room(bounds));
                    DrawRoom(bounds);
                    placed = true;
                }
            }
        }
    }

    bool Overlaps(RectInt a, RectInt b, int padding)
    {
        return a.x - padding < b.x + b.width + padding &&
               a.x + a.width + padding > b.x - padding &&
               a.y - padding < b.y + b.height + padding &&
               a.y + a.height + padding > b.y - padding;
    }

    void DrawRoom(RectInt bounds)
    {
        for (int x = bounds.x; x < bounds.x + bounds.width; x++)
        {
            for (int y = bounds.y; y < bounds.y + bounds.height; y++)
            {
                floorMap.SetTile(new Vector3Int(x, y, 0), floorTile);
            }
        }
    }

    void ConnectAllRooms()
    {
        foreach (Room room in rooms)
        {
            var closestNeighbors = rooms
                .Where(other => other != room)
                .OrderBy(other => Vector2Int.Distance(room.Center, other.Center))
                .Take(2);

            foreach (Room neighbor in closestNeighbors)
            {
                if (!room.neighbors.Contains(neighbor))
                {
                    ConnectRoomsWithCorners(room, neighbor);
                    room.neighbors.Add(neighbor);
                    neighbor.neighbors.Add(room);
                }
            }
        }
        
        EnsureAllRoomsConnected();
    }

    void ConnectRoomsWithCorners(Room a, Room b)
    {
        Vector2Int start = a.Center;
        Vector2Int end = b.Center;
        Vector2Int corner = new Vector2Int(end.x, start.y);
        
        DrawCorridorLine(start, corner, true);
        
        DrawCorridorLine(corner, end, false);
        
        DrawCorner(corner);
    }

    void DrawCorridorLine(Vector2Int start, Vector2Int end, bool isHorizontal)
    {
        Vector2Int direction = (end - start);
        direction = new Vector2Int(
            direction.x > 0 ? 1 : direction.x < 0 ? -1 : 0,
            direction.y > 0 ? 1 : direction.y < 0 ? -1 : 0);

        Vector2Int current = start;
        while (current != end)
        {
            DrawCorridorPoint(current, isHorizontal);
            current += direction;
        }
        DrawCorridorPoint(end, isHorizontal);
    }

    void DrawCorridorPoint(Vector2Int point, bool isHorizontal)
    {
        int halfWidth = corridorWidth / 2;
        for (int w = -halfWidth; w <= halfWidth; w++)
        {
            Vector3Int pos = isHorizontal ?
                new Vector3Int(point.x, point.y + w, 0) :
                new Vector3Int(point.x + w, point.y, 0);

            if (!floorMap.HasTile(pos))
            {
                floorMap.SetTile(pos, corridorTile ?? floorTile);
            }
        }
    }

    void DrawCorner(Vector2Int corner)
    {
        int halfWidth = corridorWidth / 2;
        for (int x = -halfWidth; x <= halfWidth; x++)
        {
            for (int y = -halfWidth; y <= halfWidth; y++)
            {
                Vector3Int pos = new Vector3Int(corner.x + x, corner.y + y, 0);
                if (!floorMap.HasTile(pos))
                {
                    floorMap.SetTile(pos, corridorTile ?? floorTile);
                }
            }
        }
    }

    void EnsureAllRoomsConnected()
    {
        if (rooms.Count == 0) return;

        HashSet<Room> visited = new HashSet<Room>();
        Queue<Room> queue = new Queue<Room>();
        queue.Enqueue(rooms[0]);

        while (queue.Count > 0)
        {
            Room current = queue.Dequeue();
            visited.Add(current);

            foreach (Room neighbor in current.neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                }
            }
        }

        if (visited.Count < rooms.Count)
        {
            foreach (Room room in rooms.Where(r => !visited.Contains(r)))
            {
                Room closest = visited.OrderBy(r => Vector2Int.Distance(room.Center, r.Center)).First();
                ConnectRoomsWithCorners(room, closest);
                room.neighbors.Add(closest);
                closest.neighbors.Add(room);
                visited.Add(room);
            }
        }
    }

    void CreateWalls()
    {
        HashSet<Vector3Int> floorTiles = new HashSet<Vector3Int>();

        foreach (var pos in floorMap.cellBounds.allPositionsWithin)
        {
            if (floorMap.HasTile(pos))
            {
                floorTiles.Add(pos);
            }
        }

        foreach (var pos in floorTiles)
        {
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0) continue;
                    Vector3Int wallPos = new Vector3Int(pos.x + x, pos.y + y, 0);
                    if (!floorTiles.Contains(wallPos))
                    {
                        wallMap.SetTile(wallPos, wallTile);
                    }
                }
            }
        }
    }
    
    public void SpawnPlayerInRandomRoom()
    {
        if (rooms.Count == 0 || playerPrefab == null)
        {
            Debug.LogError("No rooms or player prefab assigned!");
            return;
        }

        
        
        // Выбор случайной комнаты
        Room spawnRoom = rooms[Random.Range(0, rooms.Count)];
        Vector3 spawnPosition = new Vector3(spawnRoom.Center.x, spawnRoom.Center.y, 0);

        // Спавн игрока
        Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
    
        Debug.Log($"Player spawned in room at {spawnPosition}");
    }
}