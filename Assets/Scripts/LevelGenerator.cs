using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Linq;

public class DungeonGenerator : MonoBehaviour
{
    [Header("Dungeon settings")] [SerializeField]
    private int roomCount;

    [SerializeField] private Vector2Int minRoomSize;
    [SerializeField] private Vector2Int maxRoomSize;
    [SerializeField] private int dungeonSize;
    [SerializeField] private TileBase floorTile;
    [SerializeField] private TileBase wallTile;
    [SerializeField] private TileBase corridorTile;
    [SerializeField] private Tilemap floorMap;
    [SerializeField] private Tilemap wallMap;
    [SerializeField] private GameObject[] items;

    [Header("Creature settings")] [SerializeField]
    private GameObject playerPrefab;

    [Header("Teleport Settings")] [SerializeField]
    private GameObject portalPrefab;

    [SerializeField] private string teleportSceneName;

    [Header("Furniture settings")] [SerializeField]
    private GameObject[] furniturePrefabs;
    [SerializeField, Range(0, 5)] private int minFurniturePerRoom = 1;
    [SerializeField, Range(0, 5)] private int maxFurniturePerRoom = 3;

    [Header("Enemies settings")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField, Range(0, 10)] private int enemiesNumber;
    
    
    private readonly List<Room> rooms = new();
    private readonly System.Random random = new();
    private HashSet<Room> availableRooms;
    private int width;
    private int height;
    private const int MapPadding = 20;
    private Room playerRoom;

    private class Room
    {
        public RectInt Bounds;
        public Vector2Int Center => new(Bounds.x + Bounds.width / 2, Bounds.y + Bounds.height / 2);
        public readonly List<Room> Neighbors = new();

        public Room(RectInt bounds)
        {
            Bounds = bounds;
        }
    }

    private void Start()
    {
        GenerateDungeon();
        SpawnPlayerInRandomRoom();
        SpawnEnemiesInRandomRoom();
        SpawnItems();
        SpawnFurnitureInRooms();
        SpawnPortal();
    }

    private void GenerateDungeon()
    {
        ClearMaps();
        GenerateRooms();
        ConnectAllRooms();
        CreateWalls();
    }

    private void ClearMaps()
    {
        floorMap.ClearAllTiles();
        wallMap.ClearAllTiles();
        rooms.Clear();
    }

    private void GenerateRooms()
    {
        for (var i = 0; i < roomCount; i++)
        {
            var attempts = 0;
            var placed = false;
            while (!placed && attempts < 100)
            {
                attempts++;
                width = random.Next(minRoomSize.x, maxRoomSize.x + 1);
                height = random.Next(minRoomSize.y, maxRoomSize.y + 1);
                var x = random.Next(0, dungeonSize - width);
                var y = random.Next(0, dungeonSize - height);
                var bounds = new RectInt(x, y, width, height);
                if (!rooms.Any(r => Overlaps(r.Bounds, bounds, 2)))
                {
                    rooms.Add(new Room(bounds));
                    DrawRoom(bounds);
                    placed = true;
                }
            }
        }

        availableRooms = rooms.ToHashSet();
    }

    private static bool Overlaps(RectInt a, RectInt b, int padding)
    {
        return a.x - padding < b.x + b.width + padding &&
               a.x + a.width + padding > b.x - padding &&
               a.y - padding < b.y + b.height + padding &&
               a.y + a.height + padding > b.y - padding;
    }

    private void DrawRoom(RectInt bounds)
    {
        for (var x = bounds.x; x < bounds.x + bounds.width; x++)
        for (var y = bounds.y; y < bounds.y + bounds.height; y++)
            floorMap.SetTile(new Vector3Int(x, y, 0), floorTile);
    }

    private void ConnectAllRooms()
    {
        foreach (var room in rooms)
        {
            var closestNeighbors = rooms
                .Where(other => other != room)
                .OrderBy(other => Vector2Int.Distance(room.Center, other.Center))
                .Take(2);
            foreach (var neighbor in closestNeighbors)
            {
                if (!room.Neighbors.Contains(neighbor))
                {
                    ConnectRoomsWithCorners(room, neighbor);
                    room.Neighbors.Add(neighbor);
                    neighbor.Neighbors.Add(room);
                }
            }
        }

        EnsureAllRoomsConnected();
    }

    private void ConnectRoomsWithCorners(Room a, Room b)
    {
        var start = a.Center;
        var end = b.Center;
        var corner = new Vector2Int(end.x, start.y);
        DrawCorridorLine(start, corner, true);
        DrawCorridorLine(corner, end, false);
        DrawCorner(corner);
    }

    private void DrawCorridorLine(Vector2Int start, Vector2Int end, bool isHorizontal)
    {
        var direction = (end - start);
        direction = new Vector2Int(
            direction.x > 0 ? 1 : direction.x < 0 ? -1 : 0,
            direction.y > 0 ? 1 : direction.y < 0 ? -1 : 0);
        var current = start;
        while (current != end)
        {
            DrawCorridorPoint(current, isHorizontal);
            current += direction;
        }

        DrawCorridorPoint(end, isHorizontal);
    }

    private void DrawCorridorPoint(Vector2Int point, bool isHorizontal)
    {
        var halfWidth = 1;
        for (var w = -halfWidth; w <= halfWidth; w++)
        {
            var pos = isHorizontal
                ? new Vector3Int(point.x, point.y + w, 0)
                : new Vector3Int(point.x + w, point.y, 0);
            if (!floorMap.HasTile(pos))
                floorMap.SetTile(pos, corridorTile ?? floorTile);
        }
    }

    private void DrawCorner(Vector2Int corner)
    {
        var halfWidth = 1;
        for (var x = -halfWidth; x <= halfWidth; x++)
        {
            for (var y = -halfWidth; y <= halfWidth; y++)
            {
                var pos = new Vector3Int(corner.x + x, corner.y + y, 0);
                if (!floorMap.HasTile(pos))
                    floorMap.SetTile(pos, corridorTile ?? floorTile);
            }
        }
    }

    private void EnsureAllRoomsConnected()
    {
        if (rooms.Count == 0) return;
        var visited = new HashSet<Room>();
        var queue = new Queue<Room>();
        queue.Enqueue(rooms[0]);
        while (queue.Count > 0)
        {
            var current = queue.Dequeue();
            visited.Add(current);
            foreach (var neighbor in current.Neighbors.Where(neighbor => !visited.Contains(neighbor)))
                queue.Enqueue(neighbor);
        }

        if (visited.Count < rooms.Count)
        {
            foreach (var room in rooms.Where(r => !visited.Contains(r)))
            {
                var closest = visited.OrderBy(r => Vector2Int.Distance(room.Center, r.Center)).First();
                ConnectRoomsWithCorners(room, closest);
                room.Neighbors.Add(closest);
                closest.Neighbors.Add(room);
                visited.Add(room);
            }
        }
    }

    private void CreateWalls()
    {
        var floorTiles = new HashSet<Vector3Int>();
        foreach (var pos in floorMap.cellBounds.allPositionsWithin)
            if (floorMap.HasTile(pos))
                floorTiles.Add(pos);
        for (var x = -MapPadding; x < dungeonSize + MapPadding; x++)
        {
            for (var y = -MapPadding; y < dungeonSize + MapPadding; y++)
            {
                var pos = new Vector3Int(x, y, 0);
                if (!floorTiles.Contains(pos))
                    wallMap.SetTile(pos, wallTile);
            }
        }

        foreach (var pos in floorTiles)
            for (var x = -1; x <= 1; x++)
            for (var y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) continue;
                var wallPos = new Vector3Int(pos.x + x, pos.y + y, 0);
                if (wallPos.x >= -MapPadding && wallPos.x < dungeonSize + MapPadding &&
                    wallPos.y >= -MapPadding && wallPos.y < dungeonSize + MapPadding &&
                    !floorTiles.Contains(wallPos))
                {
                    wallMap.SetTile(wallPos, wallTile);
                }
            }
    }

    private void SpawnPlayerInRandomRoom()
    {
        if (rooms.Count == 0 || playerPrefab == null) return;
        var playerRoomSpawn = Random.Range(0, rooms.Count);
        var spawnRoom = rooms[playerRoomSpawn];
        playerRoom = spawnRoom;
        availableRooms.Remove(spawnRoom);
        var spawnPosition = new Vector3(spawnRoom.Center.x, spawnRoom.Center.y, 0);
        var player = Instantiate(playerPrefab, spawnPosition, Quaternion.identity);
        GameObject.FindGameObjectWithTag("MainCamera").transform.position = spawnPosition + new Vector3(0, 0, -10);
        CameraFollower.Target = player.transform;
        InventoryHandler.Target = player.transform;
    }

    private void SpawnEnemiesInRandomRoom()
    {
        var spawnedEnemies = 0;
        while (spawnedEnemies < enemiesNumber)
        {
            if (rooms.Count == 0 || enemyPrefab == null) return;
            var enemyRoom = Random.Range(0, availableRooms.Count);
            var enemySpawnRoom = availableRooms.ElementAt(enemyRoom);
            var enemySpawnPosition = new Vector3(enemySpawnRoom.Center.x, enemySpawnRoom.Center.y, 0);
            Instantiate(enemyPrefab, enemySpawnPosition, Quaternion.identity);
            spawnedEnemies++;
            availableRooms.Remove(enemySpawnRoom);

        }
    }

    private void SpawnItems()
    {
        for (var i = 0; i < 25; i++)
        {
            var room = rooms[random.Next(0, rooms.Count)];
            if (room == playerRoom)
            {
                i--;
                continue;
            }

            var initialX = room.Center +
                           new Vector2Int(random.Next(-room.Bounds.width / 2 + 1, room.Bounds.width / 2 - 1), 0);
            var initialY = room.Center +
                           new Vector2Int(0, random.Next(-room.Bounds.height / 2 + 1, room.Bounds.height / 2 - 1));
            var position = new Vector3Int(initialX.x, initialY.y, 0);
            var itemIndex = Random.Range(0, 3);
            Instantiate(items[itemIndex], position, Quaternion.identity);
        }
    }

    private Room FindFinalRoom()
    {
        if (rooms.Count == 0 || playerRoom == null) return null;

        Room farthestRoom = null;
        float maxDistance = 0f;

        foreach (var room in rooms)
        {
            if (room == playerRoom) continue;

            float distance = Vector2Int.Distance(playerRoom.Center, room.Center);
            if (distance > maxDistance)
            {
                maxDistance = distance;
                farthestRoom = room;
            }
        }

        return farthestRoom;
    }

    private void SpawnPortal()
    {
        Room farthestRoom = FindFinalRoom();
        if (farthestRoom != null && portalPrefab != null)
        {
            Vector3 spawnPos = new Vector3(farthestRoom.Center.x, farthestRoom.Center.y, 0);
            GameObject portal = Instantiate(portalPrefab, spawnPos, Quaternion.identity);

            Portal teleporter = portal.GetComponent<Portal>();
            if (teleporter != null)
            {
                teleporter.currentLevel = teleportSceneName;
            }
        }
    }

    private void SpawnFurnitureInRooms()
    {
        foreach (var room in rooms)
        {
            if (room == playerRoom) continue;
            var furnitureCount = random.Next(minFurniturePerRoom, maxFurniturePerRoom + 1);
            for (var i = 0; i < furnitureCount; i++)
            {
                var furniturePrefab = furniturePrefabs[random.Next(0, furniturePrefabs.Length)];
                var position = new Vector2Int(
                    random.Next(room.Bounds.x + 1, room.Bounds.x + room.Bounds.width - 1),
                    random.Next(room.Bounds.y + 1, room.Bounds.y + room.Bounds.height - 1)
                );
                var colliders = Physics2D.OverlapCircleAll(position, 1f);
                if (colliders.Length == 0)
                    Instantiate(furniturePrefab, new Vector3(position.x, position.y, 0), Quaternion.identity);
            }
        }
    }
}